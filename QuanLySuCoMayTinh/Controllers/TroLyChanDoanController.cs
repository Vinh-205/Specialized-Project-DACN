using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Data;
using QuanLySuCoMayTinh.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.ML;

namespace QuanLySuCoMayTinh.Controllers
{
    public partial class TroLyChanDoanController : Controller
    {
        private static uint[]? _labelCache;
        private static DateTime _labelCacheStamp;

        private readonly ApplicationDbContext _context;
        private readonly Services.TrieuChungMatcher _matcher;
        private readonly Services.ChanDoanAIService _ai;

        public TroLyChanDoanController(ApplicationDbContext context, Services.TrieuChungMatcher matcher, Services.ChanDoanAIService ai)
        {
            _context = context;
            _matcher = matcher;
            _ai = ai;
        }

        // GET: gợi ý triệu chứng từ mô tả tự do (tiếng Việt hoặc tiếng Anh); KTV vẫn là người tích chọn.
        [HttpGet]
        public async Task<IActionResult> GoiYTrieuChung(string? moTa)
        {
            if ((moTa?.Length ?? 0) > 2000) return BadRequest();
            if (!_matcher.SanSang) return Json(new { sanSang = false, ketQua = Array.Empty<object>() });
            var dm = (await _context.TrieuChungs.AsNoTracking().ToListAsync()).Select(t => (t.MaTC, t.TenTC, (string?)t.MoTa)).ToList();
            var khop = _matcher.GoiY(moTa, dm);
            var kq = khop.Select(x => new { ma = x.MaTC, ten = x.Ten, doTuongDong = Math.Round(x.DoTuongDong, 3) });
            var nhom = _ai.Top3Nhom(moTa).Select(n => new { ma = n.Ma, ten = n.Ten, xacSuat = Math.Round(n.XacSuat, 3) });
            return Json(new { sanSang = true, ketQua = kq, nhomLoi = nhom });
        }

        // GET: Hiển thị giao diện Trợ lý chẩn đoán
        public async Task<IActionResult> Index(int? maPhieu)
        {
            // 1. Lôi mấy cái Phiếu đang "Chờ khám" ra cho KTV chọn
            ViewBag.DanhSachPhieu = await _context.PhieuTiepNhans
                .Include(p => p.ThietBi)
                .ThenInclude(t => t.KhachHang)
                .Where(p => p.TrangThai == "Chờ khám" || p.TrangThai == "Đang kiểm tra")
                .OrderBy(p => p.NgayNhan)
                .ToListAsync();

            // 2. Lôi hết nguyên cái Từ điển Triệu chứng ra để làm mấy cái checkbox
            var danhSachTrieuChung = await _context.TrieuChungs.ToListAsync();

            ViewBag.SelectedMaPhieu = maPhieu;
            
            return View(danhSachTrieuChung);
        }

        // POST: Xử lý chẩn đoán dựa trên triệu chứng được chọn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XuLyChanDoan(int maPhieu, int[] selectedTrieuChungs, string? moTaTuDo = null)
        {
            var phieu = await _context.PhieuTiepNhans
                .Include(p => p.ThietBi)
                    .ThenInclude(t => t.KhachHang)
                .Include(p => p.User_KTV)
                .FirstOrDefaultAsync(p => p.MaPhieu == maPhieu);

            if (phieu == null) return NotFound();

            if (selectedTrieuChungs == null || selectedTrieuChungs.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng chọn ít nhất 1 triệu chứng để chẩn đoán.");
                return await Index(maPhieu);
            }

            var loiTheoDiem = await _context.TrieuChungLois
                .Where(tl => selectedTrieuChungs.Contains(tl.MaTC))
                .GroupBy(tl => tl.MaLoi)
                .Select(g => new { MaLoi = g.Key, Diem = g.Count() })
                .OrderByDescending(x => x.Diem).ThenBy(x => x.MaLoi)
                .ToListAsync();

            var maLois = loiTheoDiem.Select(x => x.MaLoi).ToList();

            var danhSachLoi = await _context.MaLois
                .Include(ml => ml.GiaiPhaps)
                .Where(ml => maLois.Contains(ml.MaLoiID))
                .ToListAsync();

            var mapDiem = loiTheoDiem.ToDictionary(x => x.MaLoi, x => x.Diem);
            var goiY = danhSachLoi
                .OrderByDescending(ml => mapDiem.TryGetValue(ml.MaLoiID, out var d) ? d : 0).ThenBy(ml => ml.MaLoiID)
                .Select(ml => new Models.ViewModels.GoiYLoiViewModel
                {
                    MaLoiID = ml.MaLoiID,
                    TenLoi = ml.TenLoi,
                    PhanLoai = ml.PhanLoai,
                    Diem = mapDiem.TryGetValue(ml.MaLoiID, out var d) ? d : 0,
                    GiaiPhaps = ml.GiaiPhaps?.ToList() ?? new List<GiaiPhap>()
                })
                .ToList();

            var trieuChungDaChon = await _context.TrieuChungs
                .Where(tc => selectedTrieuChungs.Contains(tc.MaTC))
                .ToListAsync();

            // --- LÕI HỌC MÁY: khi KTV nhập mô tả, xếp hạng mã lỗi = số triệu chứng khớp + 2 × tương đồng nhóm lỗi do mô hình dự đoán ---
            var nhomLoiAI = new List<Services.NhomLoiDuDoan>();
            if (_ai.SanSang && !string.IsNullOrWhiteSpace(moTaTuDo) && moTaTuDo.Length <= 2000)
            {
                var tatCaLoi = await _context.MaLois.Include(ml => ml.GiaiPhaps).AsNoTracking().ToListAsync();
                var lienKet = await _context.TrieuChungLois.AsNoTracking().ToListAsync();
                var tcCuaLoi = lienKet.GroupBy(x => x.MaLoi).ToDictionary(g => g.Key, g => g.Select(x => x.MaTC).ToHashSet());
                var xh = _ai.XepHang(moTaTuDo, selectedTrieuChungs, tatCaLoi.ToDictionary(l => l.MaLoiID, l => l.TenLoi), tcCuaLoi).Take(5).ToList();
                var byId = tatCaLoi.ToDictionary(l => l.MaLoiID);
                goiY = xh.Select(x => new Models.ViewModels.GoiYLoiViewModel
                {
                    MaLoiID = x.MaLoi, TenLoi = byId[x.MaLoi].TenLoi, PhanLoai = byId[x.MaLoi].PhanLoai, Diem = x.SoTrieuChungKhop,
                    DoTuongDongNhomAI = Math.Round(x.DoTuongDongNhom, 3), XacSuatMaLoiAI = Math.Round(x.XacSuatTrucTiep, 3), SoTrieuChungKhop = x.SoTrieuChungKhop,
                    GiaiPhaps = byId[x.MaLoi].GiaiPhaps?.ToList() ?? new List<GiaiPhap>()
                }).ToList();
                if (goiY.Count > 0 && xh.Count > 1)
                {
                    var bien = xh[0].Diem - xh[1].Diem; goiY[0].BienDiemAI = Math.Round(bien, 2);
                    var (mucTc, pDung) = _ai.DoTinCay(xh, moTaTuDo, selectedTrieuChungs.Length); goiY[0].DoTinCayAI = mucTc; goiY[0].XacSuatDungAI = pDung.HasValue ? Math.Round(pDung.Value, 3) : null;
                }
                nhomLoiAI = _ai.Top3Nhom(moTaTuDo);
            }

            // --- HYBRID AI: GỌI ML.NET ---
            var features = new float[500];
            foreach (var tc in selectedTrieuChungs)
            {
                if (tc > 0 && tc < 500) features[tc] = 1.0f;
            }
            Models.ML.ChanDoanPrediction? mlPrediction = null;
            uint[] classLabels = null;

            try
            {
                var predictionEngine = HttpContext.RequestServices.GetService<Microsoft.Extensions.ML.PredictionEnginePool<Models.ML.ChanDoanData, Models.ML.ChanDoanPrediction>>();
                if (predictionEngine != null)
                {
                    var data = new Models.ML.ChanDoanData { Features = features };
                    mlPrediction = predictionEngine.Predict("DiagnosticModel", data);

                    // Lấy KeyValues (mapping từ index của mảng Score sang MaLoiThucTe)
                    var modelPath = System.IO.Path.Combine(Services.ModelPaths.Root(HttpContext.RequestServices.GetRequiredService<IConfiguration>(), Directory.GetCurrentDirectory()), "ActiveModel.zip");
                    if (System.IO.File.Exists(modelPath))
                    {
                        // Thứ tự lớp lấy từ chú giải TrainingLabelValues của cột Score trong LƯỢC ĐỒ ĐẦU RA của mô hình (Model.Load chỉ trả lược đồ đầu vào).
                        // Cache theo thời điểm sửa file ActiveModel.zip để không nạp lại mô hình ở mỗi lượt chẩn đoán.
                        var stamp = System.IO.File.GetLastWriteTimeUtc(modelPath);
                        if (_labelCacheStamp != stamp || _labelCache == null)
                        {
                            var mlContext = new Microsoft.ML.MLContext();
                            var activeModel = mlContext.Model.Load(modelPath, out var inSchema);
                            var outSchema = activeModel.GetOutputSchema(inSchema);
                            Microsoft.ML.Data.VBuffer<uint> trainingLabels = default;
                            outSchema["Score"].Annotations.GetValue("TrainingLabelValues", ref trainingLabels);
                            _labelCache = trainingLabels.DenseValues().ToArray();
                            _labelCacheStamp = stamp;
                        }
                        classLabels = _labelCache;
                    }
                }
            }
            catch (Exception ex)
            {
                // Fallback: nếu không có/không đọc được ActiveModel.zip thì dùng Rule-based; ghi log để không nuốt lỗi im lặng.
                Console.Error.WriteLine("[AI] Không dùng được mô hình đang kích hoạt: " + ex.Message);
            }

            if (mlPrediction != null && classLabels != null && mlPrediction.Scores != null)
            {
                // Extract Top 3 predictions
                var topPredictions = mlPrediction.Scores
                    .Select((score, index) => new Tuple<int, float>((int)classLabels[index], score))
                    .OrderByDescending(x => x.Item2)
                    .Take(3)
                    .ToList();

                // Assign to ViewBag for UI
                ViewBag.TopAIPredictions = topPredictions;
                var activeInfo = await _context.MLModelInfos.AsNoTracking().FirstOrDefaultAsync(m => m.DangSuDung);
                bool aiKhoiTao = activeInfo?.PhienBanBoDuLieu != null && activeInfo.PhienBanBoDuLieu.StartsWith("KHOI_TAO_TONG_HOP");
                ViewBag.AIKhoiTao = aiKhoiTao;

                // Mô hình KHỞI TẠO (huấn luyện từ dữ liệu tổng hợp, chưa kiểm chứng bằng ca thật) chỉ HIỂN THỊ để tham khảo:
                // không cộng điểm, không chèn thêm gợi ý ngoài Rule và không ảnh hưởng mã lỗi chẩn đoán được lưu vào phiếu.
                var predictedMaLoi = (int)mlPrediction.PredictedMaLoi;
                var existInGoiY = goiY.FirstOrDefault(x => x.MaLoiID == predictedMaLoi);
                if (existInGoiY != null)
                {
                    if (!aiKhoiTao) existInGoiY.Diem += 100; // Tăng điểm tuyệt đối để đẩy lên đầu (chỉ với mô hình đã kiểm chứng)
                    existInGoiY.IsAIPredicted = true;
                }
                else if (!aiKhoiTao)
                {
                    var mlLoi = await _context.MaLois.Include(x => x.GiaiPhaps).FirstOrDefaultAsync(x => x.MaLoiID == predictedMaLoi);
                    if (mlLoi != null)
                    {
                        goiY.Insert(0, new Models.ViewModels.GoiYLoiViewModel
                        {
                            MaLoiID = mlLoi.MaLoiID,
                            TenLoi = mlLoi.TenLoi,
                            PhanLoai = mlLoi.PhanLoai,
                            Diem = 100, // Điểm ảo để đứng đầu
                            IsAIPredicted = true,
                            GiaiPhaps = mlLoi.GiaiPhaps?.ToList() ?? new List<GiaiPhap>()
                        });
                    }
                }
                goiY = goiY.OrderByDescending(x => x.Diem).ThenBy(x => x.MaLoiID).ToList();
            }

            // Cập nhật phiếu: lưu lỗi chẩn đoán (top 1) + chuyển trạng thái
            var top = goiY.FirstOrDefault();
            if (top != null)
            {
                phieu.MaLoi_CD = top.MaLoiID;
                // Chỉ lưu mã lỗi để tham khảo, không tự động chuyển trạng thái thành "Đang sửa chữa" ở bước này.
                // Trạng thái sẽ được chuyển khi KTV bấm "Chốt đơn & Sửa chữa".
                _context.Update(phieu);
                await _context.SaveChangesAsync();
            }

            // AI TỰ CHỐT: khi có mô tả, độ tin cậy Cao (biên điểm ≥ 0,8; đo ~91% đúng Top-1 trên tập tự soạn) và phiếu chưa sửa chữa thì tự chốt như khi KTV bấm "Chốt & Sửa chữa".
            // Tắt bằng cấu hình AI:TuDongChot=false. Có ghi nhật ký (LoaiSuKien = AI_TU_CHOT).
            if (top != null && top.DoTinCayAI == "Cao" && !string.IsNullOrWhiteSpace(moTaTuDo)
                && HttpContext.RequestServices.GetRequiredService<IConfiguration>().GetValue<bool>("AI:TuDongChot", true)
                && (phieu.TrangThai == "Chờ khám" || phieu.TrangThai == "Đang kiểm tra")
                && !(await TinhThongKeTuChot()).TamDung)
            {
                return await ChotDonSuaChua(phieu.MaPhieu, top.MaLoiID, top.TenLoi, trieuChungDaChon.Select(t => t.MaTC).ToArray(), trieuChungDaChon.Select(t => t.TenTC).ToArray(), true, top.BienDiemAI, top.XacSuatDungAI);
            }

            var vm = new Models.ViewModels.KetQuaChanDoanViewModel
            {
                MaPhieu = phieu.MaPhieu,
                Phieu = phieu,
                TrieuChungs = trieuChungDaChon,
                GoiYLoi = goiY,
                NhomLoiAI = nhomLoiAI,
                MoTaTuDo = string.IsNullOrWhiteSpace(moTaTuDo) ? null : moTaTuDo
            };

            return View("KetQua", vm);
        }

        public sealed record ThongKeTuChot(int SoCaTuChot, int SoCaBiSuaLai, double TiLeSuaLai, bool TamDung, string LyDo);
        private const int MinCaGiamSat = 10; private const double NguongSuaLai = 0.30;

        /// <summary>
        /// Giám sát AI tự chốt từ nhật ký (không cần người chấm): một ca tự chốt bị coi là "sửa lại" nếu mã lỗi hiện tại của phiếu khác mã AI chốt,
        /// hoặc nguyên nhân thực tế đã xác minh khác mã AI chốt. Xét 100 ca tự chốt gần nhất; từ 10 ca trở lên mà tỉ lệ sửa lại &gt; 30% thì TỰ TẠM DỪNG tự chốt.
        /// </summary>
        private async Task<ThongKeTuChot> TinhThongKeTuChot()
        {
            var ev = await _context.LichSuTrangThais.AsNoTracking().Where(x => x.LoaiSuKien == "AI_TU_CHOT").OrderByDescending(x => x.ThoiGian).Take(100).ToListAsync();
            int sua = 0, tong = 0;
            foreach (var e in ev)
            {
                int? maAI = null;
                try { using var d = System.Text.Json.JsonDocument.Parse(e.DuLieuBoSungJSON ?? "{}"); if (d.RootElement.TryGetProperty("MaLoi", out var m) && m.ValueKind == System.Text.Json.JsonValueKind.Number) maAI = m.GetInt32(); } catch { }
                if (maAI == null) continue;
                tong++;
                var maHienTai = await _context.PhieuTiepNhans.AsNoTracking().Where(p => p.MaPhieu == e.MaPhieu).Select(p => p.MaLoi_CD).FirstOrDefaultAsync();
                var thucTe = await _context.LichSuChanDoans.AsNoTracking().Where(h => h.MaPhieu == e.MaPhieu && h.MaLoiThucTe != null).Select(h => h.MaLoiThucTe).ToListAsync();
                if ((maHienTai != null && maHienTai != maAI) || thucTe.Any(t => t != maAI)) sua++;
            }
            double ti = tong == 0 ? 0 : (double)sua / tong; bool dung = tong >= MinCaGiamSat && ti > NguongSuaLai;
            return new ThongKeTuChot(tong, sua, Math.Round(ti, 3), dung, dung ? $"Tỉ lệ sửa lại {ti:P0} > {NguongSuaLai:P0} trên {tong} ca: tạm dừng tự chốt, quay về hiện kết quả cho KTV chọn" : "Đang hoạt động");
        }

        [HttpGet]
        [Authorize(Roles = "Admin KTV")]
        public async Task<IActionResult> ThongKeTuChotAI() => Json(await TinhThongKeTuChot());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChotDonSuaChua(int MaPhieu, int? MaLoi, string? TenLoi, int[]? TrieuChungIds, string[]? TrieuChungNames, bool tuDongAI = false, double? bienDiemAI = null, double? xacSuatDungAI = null)
        {
            var phieu = await _context.PhieuTiepNhans.FindAsync(MaPhieu);
            
            if (phieu != null)
            {
                var trangThaiCu = phieu.TrangThai;

                // Bước 3: Chuyển trạng thái sang "Đang sửa chữa"
                phieu.TrangThai = "Đang sửa chữa";

                // Lưu Mã lỗi chẩn đoán từ AI
                if (MaLoi.HasValue && MaLoi.Value > 0)
                {
                    phieu.MaLoi_CD = MaLoi.Value;
                }

                // Gán KTV đang đăng nhập (không hardcode)
                var ktvId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(ktvId))
                    phieu.MaUser_KTV = int.Parse(ktvId);

                _context.Update(phieu);

                // Ghi nhật ký chuyển trạng thái
                _context.LichSuTrangThais.Add(new LichSuTrangThai
                {
                    MaPhieu = MaPhieu,
                    TrangThaiCu = trangThaiCu,
                    TrangThaiMoi = "Đang sửa chữa",
                    ThoiGian = DateTime.Now,
                    Ma_KTV = string.IsNullOrEmpty(ktvId) ? null : int.Parse(ktvId),
                    LoaiSuKien = tuDongAI ? "AI_TU_CHOT" : null,
                    DuLieuBoSungJSON = tuDongAI ? System.Text.Json.JsonSerializer.Serialize(new { MaLoi, TenLoi, BienDiem = bienDiemAI, XacSuatDungUocLuong = xacSuatDungAI, MucTinCay = "Cao", DieuKien = "xác suất đúng ước lượng ≥ 0,75", GhiChu = "AI tự chốt khi tin cậy Cao; chưa có người xác minh nguyên nhân" }) : null
                });

                // Lưu Lịch sử Chẩn đoán (Hybrid AI)
                var lsChanDoan = new LichSuChanDoan
                {
                    MaPhieu = MaPhieu,
                    TenLoiThucTeLuuTru = null, // Mã KTV chẩn đoán nằm ở phiếu; chưa có nguyên nhân được xác minh.
                    ThoiGian = DateTime.Now,
                    NguonDuLieu = QuanLySuCoMayTinh.Models.ML.TrainingDataSource.UserHistory
                };

                if (TrieuChungNames != null && TrieuChungNames.Any())
                {
                    // 1. Lấy mã lỗi RuleEngine
                    var trieuChungList = TrieuChungNames.ToList();
                    var matchedRule = await _context.TrieuChungLois
                        .Where(tl => trieuChungList.Contains(tl.TrieuChung.TenTC))
                        .GroupBy(tl => tl.MaLoi)
                        .Select(g => new { MaLoi = g.Key, Diem = g.Count() })
                        .OrderByDescending(x => x.Diem).ThenBy(x => x.MaLoi)
                        .FirstOrDefaultAsync();

                    if (matchedRule != null)
                        lsChanDoan.MaLoiRule = matchedRule.MaLoi;

                    // 2. Chạy lại AI Prediction để lấy Top-1 và Confidence
                    var features = new float[500];
                    foreach (var tc in TrieuChungIds)
                    {
                        if (tc > 0 && tc < 500) features[tc] = 1.0f;
                    }
                    try
                    {
                        var predictionEngine = HttpContext.RequestServices.GetService<Microsoft.Extensions.ML.PredictionEnginePool<Models.ML.ChanDoanData, Models.ML.ChanDoanPrediction>>();
                        if (predictionEngine != null)
                        {
                            var data = new Models.ML.ChanDoanData { Features = features };
                            var pred = predictionEngine.Predict("DiagnosticModel", data);
                            if (pred != null)
                            {
                                lsChanDoan.MaMoHinh = await _context.MLModelInfos.Where(m => m.DangSuDung && m.LoaiBaiToan == "CHAN_DOAN_TRIEU_CHUNG_MA_LOI").OrderByDescending(m => m.NgayTao).Select(m => (int?)m.MaMoHinh).FirstOrDefaultAsync();
                                lsChanDoan.MaLoiAITop1 = (int)pred.PredictedMaLoi;
                                lsChanDoan.DoTinCayTop1 = pred.Scores != null && pred.Scores.Length > 0 ? pred.Scores.Max() : 0;
                            }
                        }
                    }
                    catch (Exception) { /* Fallback */ }
                }

                _context.LichSuChanDoans.Add(lsChanDoan);
                await _context.SaveChangesAsync(); // Sinh Id cho lsChanDoan

                if (TrieuChungIds != null && TrieuChungNames != null && TrieuChungIds.Length == TrieuChungNames.Length)
                {
                    for (int i = 0; i < TrieuChungIds.Length; i++)
                    {
                        _context.LichSuChanDoanTrieuChungs.Add(new LichSuChanDoanTrieuChung
                        {
                            MaLichSuChanDoan = lsChanDoan.MaLichSuChanDoan,
                            MaTrieuChung = TrieuChungIds[i],
                            TenTrieuChungLuuTru = TrieuChungNames[i]
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["Success"] = tuDongAI
                    ? $"AI đã tự chốt mã lỗi \"{TenLoi}\" cho Phiếu #{MaPhieu.ToString("D4")} (tin cậy Cao) và chuyển sang Đang sửa chữa. KTV có thể kiểm tra lại ở chi tiết phiếu."
                    : $"Đã chốt đơn sửa chữa cho Phiếu #{MaPhieu.ToString("D4")} thành công!";
            }

            return RedirectToAction("Details", "PhieuTiepNhan", new { id = MaPhieu });
        }

        [HttpPost]
        [Authorize(Roles = "Admin KTV")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuanLuyen([FromServices] System.Threading.Channels.Channel<Services.AITrainingRequest> channel)
        {
            if (!HttpContext.RequestServices.GetRequiredService<IConfiguration>().GetValue<bool>("AITraining:Enabled", true)) return StatusCode(409, "Training đang bị tắt trong cấu hình (AITraining:Enabled=false).");
            // Kiểm tra xem có job nào đang chạy không
            var isRunning = await _context.AITrainingJobs.AnyAsync(j => j.TrangThai == Models.TrainingJobStatus.Running.ToString() || j.TrangThai == Models.TrainingJobStatus.Pending.ToString());
            
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isRunning)
            {
                if (isAjax) return Json(new { success = false, message = "Hệ thống đang huấn luyện." });
                TempData["ErrorMsg"] = "Hệ thống đang trong quá trình huấn luyện một mô hình khác. Vui lòng chờ hoàn tất trước khi yêu cầu huấn luyện mới.";
                return RedirectToAction("MLModels", "Admin"); 
            }

            var username = User.Identity?.Name ?? "System";

            var job = new Models.AITrainingJob
            {
                NgayTao = DateTime.Now,
                TrangThai = Models.TrainingJobStatus.Pending.ToString(),
                MaNguoiDung = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var jobActor) ? jobActor : null,
                ThongBaoLoi = null
            };
            _context.AITrainingJobs.Add(job);
            await _context.SaveChangesAsync();

            await channel.Writer.WriteAsync(new Services.AITrainingRequest
            {
                JobId = job.MaTacVu,
                RequestedBy = username
            });

            if (isAjax) return Json(new { success = true, message = "Đã khởi tạo tiến trình huấn luyện ngầm." });

            TempData["Success"] = $"Đã khởi tạo tiến trình huấn luyện ngầm (Job #{job.MaTacVu}). Bạn có thể theo dõi kết quả ở bảng điều khiển quản trị.";
            return RedirectToAction("MLModels", "Admin"); // TODO: Chuyển về trang quản lý Model
        }
    }
}
