using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Data;
using System.Threading.Channels;
using QuanLySuCoMayTinh.Services;
using Microsoft.Extensions.ML;
using QuanLySuCoMayTinh.Models.ML;

var builder = WebApplication.CreateBuilder(args);

// Khởi tạo file model rỗng nếu chưa có để tránh lỗi khi startup
var modelsDir = QuanLySuCoMayTinh.Services.ModelPaths.Root(builder.Configuration, builder.Environment.ContentRootPath);
if (!Directory.Exists(modelsDir)) Directory.CreateDirectory(modelsDir);
var activeModelPath = Path.Combine(modelsDir, "ActiveModel.zip");
if (!File.Exists(activeModelPath))
{
    // Tạo file zip rỗng tạm (có thể không chạy được Predict nhưng tránh crash startup)
    // Cách an toàn hơn: Chỉ AddPredictionEnginePool nếu file tồn tại
}

builder.Services.AddPredictionEnginePool<ChanDoanData, ChanDoanPrediction>()
    .FromFile(modelName: "DiagnosticModel", filePath: activeModelPath, watchForChanges: true);

// --- ĐOẠN CODE KẾT NỐI DATABASE MÌNH THÊM VÀO ĐÂY NÈ ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// -------------------------------------------------------

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cấu hình Cookie Authentication cho toàn bộ hệ thống
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
    });

// Mô hình song ngữ (ONNX Runtime, chạy trong website). Thiếu tệp thì SanSang=false và hệ thống dùng luật từ khóa.
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var dir = QuanLySuCoMayTinh.Services.ModelPaths.SongNgu(cfg, Directory.GetCurrentDirectory());
    return new QuanLySuCoMayTinh.Services.SongNguClassifier(dir, cfg["AI:SongNguModel"] ?? "song_ngu_int8.onnx");
});
builder.Services.AddSingleton<QuanLySuCoMayTinh.Services.TrieuChungMatcher>();
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    return new QuanLySuCoMayTinh.Services.MaLoiTrucTiepClassifier(sp.GetRequiredService<QuanLySuCoMayTinh.Services.SongNguClassifier>(),
        Path.Combine(QuanLySuCoMayTinh.Services.ModelPaths.Root(cfg, Directory.GetCurrentDirectory()), "MaLoiTrucTiep"));
});
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    return new QuanLySuCoMayTinh.Services.KetQuaSuaClassifier(sp.GetRequiredService<QuanLySuCoMayTinh.Services.SongNguClassifier>(),
        Path.Combine(QuanLySuCoMayTinh.Services.ModelPaths.Root(cfg, Directory.GetCurrentDirectory()), "KetQuaSua"));
});
builder.Services.AddSingleton<QuanLySuCoMayTinh.Services.ChanDoanAIService>();
builder.Services.AddSingleton(Channel.CreateUnbounded<AITrainingRequest>());
builder.Services.AddHostedService<AITrainingBackgroundService>();

var app = builder.Build();

// --- TỰ ĐỘNG TẠO BẢNG VÀ NẠP DỮ LIỆU MỒI KHI CHẠY TRÊN SOMEE ---
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();

    // 1. Đánh dấu dữ liệu cũ là LegacyRandom
    var legacyRecords = dbContext.LichSuChanDoans
        // Chỉ dữ liệu cũ mang nguồn mặc định (UserHistory) mới bị coi là LegacyRandom; không đụng tới Synthetic/Public/RealRepair vừa nạp.
        .Where(x => x.NguonDuLieu == QuanLySuCoMayTinh.Models.ML.TrainingDataSource.UserHistory && x.MaLichSuChanDoan <= 102)
        .ToList();
    if (legacyRecords.Any())
    {
        foreach (var r in legacyRecords)
        {
            r.NguonDuLieu = QuanLySuCoMayTinh.Models.ML.TrainingDataSource.LegacyRandom;
        }
        dbContext.SaveChanges();
    }

    // 2. Tự động mồi dữ liệu giả lập cho AI (Dữ liệu SyntheticRule)
    int currentRecords = dbContext.LichSuChanDoans.Count(x => x.NguonDuLieu != QuanLySuCoMayTinh.Models.ML.TrainingDataSource.LegacyRandom);
    if (currentRecords < 500)
    {
        var maLois = dbContext.MaLois.ToList();
        var trieuChungLois = dbContext.TrieuChungLois.Include(x => x.TrieuChung).ToList();
        var allTrieuChungs = dbContext.TrieuChungs.ToList();

        // Cần ít nhất một phiếu để thỏa FK MaPhieu; database rỗng (chưa có phiếu) thì bỏ qua bước nạp dữ liệu mồi.
        if (maLois.Any() && trieuChungLois.Any() && dbContext.PhieuTiepNhans.Any())
        {
            var rand = new Random(20261105); // cố định hạt giống để dữ liệu mồi (và kết quả đánh giá) lặp lại được
            var phieus = dbContext.PhieuTiepNhans.Take(10).ToList();
            int defaultPhieuId = phieus.Any() ? phieus.First().MaPhieu : 1;

            foreach (var loi in maLois)
            {
                var correctSymptoms = trieuChungLois.Where(x => x.MaLoi == loi.MaLoiID).Select(x => x.TrieuChung).ToList();
                if (!correctSymptoms.Any()) continue;

                // Tạo khoảng 50 bản ghi Synthetic cho mỗi lỗi
                for (int i = 0; i < 50; i++)
                {
                    var ls = new QuanLySuCoMayTinh.Models.LichSuChanDoan
                    {
                        MaPhieu = phieus.Any() ? phieus[rand.Next(phieus.Count)].MaPhieu : defaultPhieuId,
                        MaLoiRule = loi.MaLoiID,
                        MaLoiThucTe = loi.MaLoiID,
                        TenLoiThucTeLuuTru = "[SYNTHETIC] " + loi.TenLoi,
                        ThoiGian = DateTime.Now.AddDays(-rand.Next(1, 30)),
                        NguonDuLieu = QuanLySuCoMayTinh.Models.ML.TrainingDataSource.SyntheticRule
                    };
                    dbContext.LichSuChanDoans.Add(ls);
                    dbContext.SaveChanges();

                    bool isNoisy = rand.Next(100) < 20; // 20% Noisy
                    int numTc = isNoisy ? correctSymptoms.Count : rand.Next(1, correctSymptoms.Count + 1);
                    var selectedTcs = correctSymptoms.OrderBy(x => rand.Next()).Take(numTc).ToList();

                    if (isNoisy)
                    {
                        // Thêm 1 triệu chứng nhiễu ngẫu nhiên
                        var noisePool = allTrieuChungs.Where(tc => !correctSymptoms.Any(c => c.MaTC == tc.MaTC)).ToList();
                        if (noisePool.Any())
                        {
                            var noiseTc = noisePool[rand.Next(noisePool.Count)];
                            selectedTcs.Add(noiseTc);
                        }
                    }

                    foreach (var tc in selectedTcs)
                    {
                        dbContext.LichSuChanDoanTrieuChungs.Add(new QuanLySuCoMayTinh.Models.LichSuChanDoanTrieuChung
                        {
                            MaLichSuChanDoan = ls.MaLichSuChanDoan,
                            MaTrieuChung = tc.MaTC,
                            TenTrieuChungLuuTru = tc.TenTC
                        });
                    }
                    dbContext.SaveChanges();
                }
            }
        }
    }
}
// --------------------------------------------------------------

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles(); // Đảm bảo phục vụ file tĩnh đúng cách trên IIS Somee
app.UseRouting();

app.UseAuthentication(); // Bắt buộc phải đặt ngay TRƯỚC UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Welcome}/{id?}");

app.Run();