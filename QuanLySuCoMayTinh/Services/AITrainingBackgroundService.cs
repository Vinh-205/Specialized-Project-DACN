using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using QuanLySuCoMayTinh.Data;
using QuanLySuCoMayTinh.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.ML;

namespace QuanLySuCoMayTinh.Services
{
    public class AITrainingRequest
    {
        public int JobId { get; set; }
        public string RequestedBy { get; set; }
    }

    public class AITrainingBackgroundService : BackgroundService
    {
        private readonly Channel<AITrainingRequest> _channel;
        private readonly ILogger<AITrainingBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private bool _isTraining = false;

        public AITrainingBackgroundService(
            Channel<AITrainingRequest> channel,
            ILogger<AITrainingBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _channel = channel;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public bool IsTraining => _isTraining;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var request in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                if (!_serviceProvider.GetRequiredService<IConfiguration>().GetValue<bool>("AITraining:Enabled", true)) continue;
                _isTraining = true;
                _logger.LogInformation($"Starting AI Training Job #{request.JobId}");

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var job = await dbContext.AITrainingJobs.FindAsync(request.JobId);
                        
                        if (job == null) continue;

                        job.NgayBatDau = DateTime.Now;
                        job.TrangThai = TrainingJobStatus.Running.ToString();
                        await dbContext.SaveChangesAsync();

                        // --- DATA PREPARATION ---
                        _logger.LogInformation("Loading training data...");
                        // 1. Lấy dữ liệu (Loại bỏ LegacyRandom)
                        var trainingDataRows = await TrainingEligibility.ForTraining(dbContext.LichSuChanDoans)
                            .Include(x => x.TrieuChungs)
                            .Include(x => x.PhieuTiepNhan)
                            .ToListAsync();

                        int realCount = trainingDataRows.Count(x => x.NguonDuLieu == QuanLySuCoMayTinh.Models.ML.TrainingDataSource.RealRepair);
                        int userCount = trainingDataRows.Count(x => x.NguonDuLieu == QuanLySuCoMayTinh.Models.ML.TrainingDataSource.UserHistory);
                        int syntheticCount = trainingDataRows.Count(x => x.NguonDuLieu == QuanLySuCoMayTinh.Models.ML.TrainingDataSource.SyntheticRule);

                        int totalRecords = trainingDataRows.Count;
                        _logger.LogInformation($"Total trainable records loaded: {totalRecords} (Real: {realCount}, User: {userCount}, Synthetic: {syntheticCount})");

                        // Chuyển đổi sang format của ML.NET kèm Weight
                        float[] CreateSymptomVector(IEnumerable<QuanLySuCoMayTinh.Models.LichSuChanDoanTrieuChung> trieuChungs)
                        {
                            var vector = new float[500];
                            foreach (var tc in trieuChungs)
                            {
                                if (tc.MaTrieuChung > 0 && tc.MaTrieuChung < 500)
                                    vector[tc.MaTrieuChung] = 1.0f;
                            }
                            return vector;
                        }

                        var trainingData = trainingDataRows.Select(x => new QuanLySuCoMayTinh.Models.ML.ChanDoanData
                        {
                            Features = CreateSymptomVector(x.TrieuChungs),
                            TrieuChungsText = string.Join(", ", x.TrieuChungs.Select(t => t.TenTrieuChungLuuTru)),
                            MaLoiThucTe = (uint)x.MaLoiThucTe.Value,
                            Weight = x.NguonDuLieu == QuanLySuCoMayTinh.Models.ML.TrainingDataSource.RealRepair ? 1.0f :
                                     x.NguonDuLieu == QuanLySuCoMayTinh.Models.ML.TrainingDataSource.UserHistory ? 0.5f : 0.2f
                        }).ToList();

                        _logger.LogInformation($"Total trainable records loaded: {totalRecords}");

                        if (totalRecords < 10)
                        {
                            throw new Exception($"Dataset quá nhỏ ({totalRecords} bản ghi). Cần tối thiểu 10 bản ghi để phân chia Train/Test và đánh giá độ chính xác hợp lệ.");
                        }

                        // Kiểm tra độ cân bằng (Class Imbalance)
                        var groupCount = trainingData.GroupBy(x => x.MaLoiThucTe).Select(g => new { Key = g.Key, Count = g.Count() }).ToList();
                        _logger.LogInformation($"Classes detected: {groupCount.Count}");

                        var imbalancedClasses = groupCount.Where(g => g.Count < 2).ToList();
                        string warningMessage = "";
                        if (imbalancedClasses.Any())
                        {
                            warningMessage = $"[WARNING] Mất cân bằng dữ liệu: {imbalancedClasses.Count} loại lỗi chỉ có 1 bản ghi thực tế. Mô hình có thể dự đoán kém với các lỗi này.";
                            _logger.LogWarning(warningMessage);
                            // Ghi log warning vào Job History (không crash)
                            job.ThongBaoLoi = warningMessage; 
                        }

                        // --- ML.NET TRAINING ---
                        _logger.LogInformation("Initializing ML.NET pipeline...");
                        var mlContext = new Microsoft.ML.MLContext(seed: 0);

                        // Only verified real repairs may enter the main evaluation set.
                        var realRows = trainingDataRows.Where(x => x.NguonDuLieu == Models.ML.TrainingDataSource.RealRepair).ToList();
                        var rng = new Random(0);
                        var groups = realRows.GroupBy(x => x.PhieuTiepNhan?.MaTB ?? x.MaPhieu).OrderBy(_ => rng.Next()).ToList();
                        bool isBootstrap = false;
                        HashSet<int> testIds;
                        if (groups.Count >= 2)
                        {
                            testIds = groups.Take(Math.Max(1, groups.Count / 5)).SelectMany(g => g).Select(x => x.MaLichSuChanDoan).ToHashSet();
                        }
                        else
                        {
                            // Chế độ KHỞI TẠO: chưa đủ ca thật đã xác minh. Cho phép huấn luyện từ dữ liệu tổng hợp/công khai để hệ thống dùng được ngay;
                            // đánh giá trên phần giữ lại của chính dữ liệu này (chia theo tổ hợp triệu chứng+nhãn để không rò rỉ bản trùng).
                            // Mô hình được gắn nhãn KHOI_TAO_TONG_HOP, KHÔNG chứng minh chất lượng trên ca thật.
                            var allowBootstrap = _serviceProvider.GetRequiredService<IConfiguration>().GetValue<bool>("AI:AllowBootstrapModel", true);
                            var pool = trainingDataRows.Where(x => x.NguonDuLieu != Models.ML.TrainingDataSource.RealRepair).ToList();
                            if (!allowBootstrap || pool.Count < 10)
                                throw new InvalidOperationException("Cần ít nhất hai nhóm thiết bị có nhãn đã xác minh; không dùng public/synthetic làm test chính.");
                            isBootstrap = true;
                            string Sig(LichSuChanDoan x) => x.MaLoiThucTe + "|" + string.Join(",", x.TrieuChungs.Select(t => t.MaTrieuChung).OrderBy(i => i));
                            var sigGroups = pool.GroupBy(Sig).OrderBy(_ => rng.Next()).ToList();
                            testIds = new HashSet<int>();
                            // Mỗi mã lỗi phải còn ít nhất một tổ hợp triệu chứng trong tập huấn luyện (nếu không, lớp đó không thể học được và phép đo bất công với AI).
                            var sigPerLabel = sigGroups.GroupBy(g => g.First().MaLoiThucTe).ToDictionary(x => x.Key, x => x.Count());
                            foreach (var g in sigGroups)
                            {
                                if (testIds.Count >= Math.Max(1, pool.Count / 5)) break;
                                var lbl = g.First().MaLoiThucTe;
                                if (sigPerLabel[lbl] <= 1) continue;
                                sigPerLabel[lbl]--;
                                foreach (var r in g) testIds.Add(r.MaLichSuChanDoan);
                            }
                            job.ThongBaoLoi = (job.ThongBaoLoi ?? "") + " [KHỞI TẠO] Huấn luyện và đánh giá trên dữ liệu tổng hợp/công khai, CHƯA kiểm chứng bằng ca thật đã xác minh.";
                        }
                        var testRows = (isBootstrap ? trainingDataRows : realRows).Where(x => testIds.Contains(x.MaLichSuChanDoan)).ToList();
                        var trainRows = trainingDataRows.Where(x => !testIds.Contains(x.MaLichSuChanDoan)).ToList();
                        Models.ML.ChanDoanData ConvertRow(LichSuChanDoan x) => new() {
                            Features = CreateSymptomVector(x.TrieuChungs), TrieuChungsText = string.Join(", ", x.TrieuChungs.Select(t => t.TenTrieuChungLuuTru)),
                            MaLoiThucTe = (uint)x.MaLoiThucTe!.Value, Weight = x.NguonDuLieu == Models.ML.TrainingDataSource.RealRepair ? 1f : x.NguonDuLieu == Models.ML.TrainingDataSource.PublicDataset ? .3f : .2f };
                        var trainSet = mlContext.Data.LoadFromEnumerable(trainRows.Select(ConvertRow));
                        var testSet = mlContext.Data.LoadFromEnumerable(testRows.Select(ConvertRow));

                        // Pipeline (gán Weight)
                        var pipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "Label", inputColumnName: nameof(QuanLySuCoMayTinh.Models.ML.ChanDoanData.MaLoiThucTe))
                            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "Label", featureColumnName: "Features", exampleWeightColumnName: "Weight"))
                            .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName: "PredictedLabel"));

                        // Train model
                        _logger.LogInformation($"Training ML model...");
                        var model = pipeline.Fit(trainSet);

                        // Evaluate model
                        _logger.LogInformation($"Evaluating ML model...");
                        var predictions = model.Transform(testSet);
                        var metrics = mlContext.MulticlassClassification.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");

                        // Calculate Rule vs AI Accuracy on Test Set manually
                        double aiCorrect = 0;
                        double ruleCorrect = 0;
                        int testCount = 0;
                        var testEnumerable = mlContext.Data.CreateEnumerable<QuanLySuCoMayTinh.Models.ML.ChanDoanData>(testSet, reuseRowObject: false).ToList();
                        
                        var predictionEngine = mlContext.Model.CreatePredictionEngine<QuanLySuCoMayTinh.Models.ML.ChanDoanData, QuanLySuCoMayTinh.Models.ML.ChanDoanPrediction>(model);
                        var allRules = await dbContext.TrieuChungLois.Include(x => x.TrieuChung).ToListAsync();
                        
                        // Thứ tự lớp lấy từ CHÍNH mô hình vừa huấn luyện (chú giải TrainingLabelValues của cột Score); nếu không đọc được thì dùng thứ tự xuất hiện
                        // đầu tiên của nhãn trong tập huấn luyện (khớp MapValueToKey mặc định). Không dùng nhãn của ca thật trong CSDL để suy ra thứ tự lớp.
                        uint[] classLabels;
                        try
                        {
                            Microsoft.ML.Data.VBuffer<uint> trainingLabels = default;
                            predictionEngine.OutputSchema["Score"].Annotations.GetValue("TrainingLabelValues", ref trainingLabels);
                            classLabels = trainingLabels.DenseValues().ToArray();
                        }
                        catch (Exception)
                        {
                            classLabels = trainRows.Select(x => (uint)x.MaLoiThucTe!.Value).Distinct().ToArray();
                        }

                        double top3Correct = 0;
                        double top5Correct = 0;
                        var confusionMatrixDict = new Dictionary<string, int>();

                        foreach (var t in testEnumerable)
                        {
                            testCount++;
                            var pred = predictionEngine.Predict(t);
                            if (pred.PredictedMaLoi == t.MaLoiThucTe) aiCorrect++;

                            if (pred.Scores != null && classLabels != null && pred.Scores.Length == classLabels.Length)
                            {
                                var sortedScores = pred.Scores
                                    .Select((score, index) => new { Score = score, MaLoi = classLabels[index] })
                                    .OrderByDescending(x => x.Score)
                                    .ToList();

                                if (sortedScores.Take(3).Any(x => x.MaLoi == t.MaLoiThucTe)) top3Correct++;
                                if (sortedScores.Take(5).Any(x => x.MaLoi == t.MaLoiThucTe)) top5Correct++;
                            }

                            // Tính Confusion Matrix
                            string cmKey = $"{t.MaLoiThucTe}_{pred.PredictedMaLoi}";
                            if (confusionMatrixDict.ContainsKey(cmKey)) confusionMatrixDict[cmKey]++;
                            else confusionMatrixDict[cmKey] = 1;

                            // Rule Engine
                            // Triệu chứng lấy từ bản ghi kiểm thử gốc (TrieuChungsText bị [NoColumn] nên KHÔNG có trong IDataView -> trước đây Rule luôn đạt 0%).
                            var symptomIds = testRows[testCount - 1].TrieuChungs.Select(x => x.MaTrieuChung).ToHashSet();
                            var matchedRules = allRules.Where(r => symptomIds.Contains(r.MaTC)).GroupBy(r => r.MaLoi).Select(g => new { MaLoi = g.Key, Count = g.Count() }).OrderByDescending(g => g.Count).FirstOrDefault();
                            if (matchedRules != null && matchedRules.MaLoi == t.MaLoiThucTe) ruleCorrect++;
                        }
                        double aiAcc = testCount > 0 ? (aiCorrect / testCount) * 100 : 0;
                        double top3Acc = testCount > 0 ? (top3Correct / testCount) * 100 : 0;
                        double top5Acc = testCount > 0 ? (top5Correct / testCount) * 100 : 0;
                        double ruleAcc = testCount > 0 ? (ruleCorrect / testCount) * 100 : 0;

                        // Save model to disk
                        var modelsDir = ModelPaths.Root(_serviceProvider.GetRequiredService<IConfiguration>(), _serviceProvider.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>().ContentRootPath);
                        if (!Directory.Exists(modelsDir)) Directory.CreateDirectory(modelsDir);

                        var modelFileName = $"DiagnosticModel_{DateTime.Now:yyyyMMddHHmmss}.zip";
                        var modelFilePath = Path.Combine(modelsDir, modelFileName);
                        
                        // Xóa các file cũ nếu cần (tùy chọn)
                        mlContext.Model.Save(model, trainSet.Schema, modelFilePath);

                        // Create MLModelInfo
                        var newModel = new MLModelInfo
                        {
                            PhienBan = $"v{DateTime.Now:yyyyMMddHHmmss}",
                            PhienBanBoDuLieu = isBootstrap ? $"KHOI_TAO_TONG_HOP_{DateTime.Now:yyyyMMddHHmmss}" : $"DS_{DateTime.Now:yyyyMMddHHmmss}",
                            NgayTaoBoDuLieu = DateTime.Now,
                            SoBanGhiThucTe = trainRows.Count(x => x.NguonDuLieu == Models.ML.TrainingDataSource.RealRepair),
                            SoBanGhiLichSuNguoiDung = userCount,
                            SoBanGhiTongHop = trainRows.Count(x => x.NguonDuLieu == Models.ML.TrainingDataSource.SyntheticRule),
                            DoChinhXacRule = ruleAcc,
                            DoChinhXacAI = aiAcc,
                            DoChinhXacTop3 = top3Acc,
                            DoChinhXacTop5 = top5Acc,
                            MaTranNhamLanJSON = System.Text.Json.JsonSerializer.Serialize(confusionMatrixDict),
                            DoChinhXacMicro = metrics.MicroAccuracy,
                            DoChinhXacMacro = metrics.MacroAccuracy,
                            LogLoss = metrics.LogLoss,
                            LogLossReduction = metrics.LogLossReduction,
                            SoBanGhiHuanLuyen = trainRows.Count,
                            NgayTao = DateTime.Now,
                            DangSuDung = false, 
                            DuongDanTep = modelFilePath
                        };
                        dbContext.MLModelInfos.Add(newModel);
                        await dbContext.SaveChangesAsync();

                        job.ThongBaoLoi = (job.ThongBaoLoi ?? "") + $" Sources: Public={trainRows.Count(x => x.NguonDuLieu == Models.ML.TrainingDataSource.PublicDataset)}; Synthetic={syntheticCount}; verified-real-test={testRows.Count}.";
                        job.MaMoHinh = newModel.MaMoHinh;
                        job.NgayHoanThanh = DateTime.Now;
                        job.TrangThai = TrainingJobStatus.Completed.ToString();
                        await dbContext.SaveChangesAsync();
                        _logger.LogInformation($"Completed AI Training Job #{request.JobId}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error during AI Training Job #{request.JobId}");
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var job = await dbContext.AITrainingJobs.FindAsync(request.JobId);
                        if (job != null)
                        {
                            job.NgayHoanThanh = DateTime.Now;
                            job.TrangThai = TrainingJobStatus.Failed.ToString();
                            job.ThongBaoLoi = ex.Message;
                            await dbContext.SaveChangesAsync();
                        }
                    }
                }
                finally
                {
                    _isTraining = false;
                }
            }
        }
    }
}
