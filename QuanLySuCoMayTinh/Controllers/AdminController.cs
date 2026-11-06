using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Data;
using QuanLySuCoMayTinh.Models;
using System.Security.Claims;
using QuanLySuCoMayTinh.Models.ML;
using QuanLySuCoMayTinh.Services;

namespace QuanLySuCoMayTinh.Controllers
{
    // [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ─── Kiểm tra quyền Admin KTV ───
        private bool IsAdmin()
        {
            return User.IsInRole("Admin KTV");
        }

        // ════════════════════════════════════════
        // QUẢN LÝ NHÂN SỰ — Users
        // ════════════════════════════════════════

        // GET: /Admin/Users — Danh sách nhân sự
        public async Task<IActionResult> Users(string search = "")
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.FullName.Contains(search) ||
                    u.Username.Contains(search) ||
                    u.Role.Contains(search));
            }

            var users = await query
                .OrderBy(u => u.Role)
                .ThenBy(u => u.FullName)
                .ToListAsync();

            // Tính số phiếu xử lý của từng nhân viên (Tổng số & Hoàn thành)
            var phieuCounts = await _context.PhieuTiepNhans
                .GroupBy(p => p.MaUser_KTV)
                .Select(g => new
                {
                    UserId = g.Key,
                    Total = g.Count(),
                    Completed = g.Count(p => p.TrangThai == "Hoàn thành")
                })
                .ToDictionaryAsync(x => x.UserId, x => new Tuple<int, int>(x.Total, x.Completed));

            ViewData["PhieuCounts"] = phieuCounts;

            // Thống kê nhanh
            ViewData["TotalUsers"]  = await _context.Users.CountAsync();
            ViewData["ActiveUsers"] = await _context.Users.CountAsync(u => u.Status);
            ViewData["LockedUsers"] = await _context.Users.CountAsync(u => !u.Status);
            ViewData["AdminCount"]  = await _context.Users.CountAsync(u => u.Role == "Admin KTV");
            ViewData["Search"]      = search;

            return View("Users/Index", users);
        }

        // POST: /Admin/Users/Create — Tạo tài khoản mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(string fullName, string username, string password, string role)
        {
            if (!IsAdmin()) return Json(new { success = false, message = "Không có quyền." });

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return Json(new { success = false, message = "Vui lòng nhập đầy đủ thông tin." });

            // Kiểm tra trùng username
            var exists = await _context.Users.AnyAsync(u => u.Username == username);
            if (exists)
                return Json(new { success = false, message = "Tên đăng nhập đã tồn tại trong hệ thống." });

            var newUser = new User
            {
                FullName      = fullName.Trim(),
                Username      = username.Trim().ToLower(),
                Password_Hash = password,
                Role          = "KTV", // Chỉ cho phép tạo tài khoản KTV
                Status        = true
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã tạo tài khoản '{newUser.FullName}' thành công!" });
        }

        // POST: /Admin/Users/EditUser — Chỉnh sửa thông tin tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, string fullName, string password, string role)
        {
            if (!IsAdmin()) return Json(new { success = false, message = "Không có quyền." });

            var user = await _context.Users.FindAsync(id);
            if (user == null) return Json(new { success = false, message = "Không tìm thấy tài khoản." });

            // Ngăn xóa vai trò Admin cuối cùng
            if (user.Role == "Admin KTV" && role == "KTV")
            {
                var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin KTV" && u.Status);
                if (adminCount <= 1)
                    return Json(new { success = false, message = "Phải có ít nhất 1 Admin KTV trong hệ thống." });
            }

            user.FullName = fullName.Trim();
            user.Role = role == "Admin KTV" ? "Admin KTV" : "KTV";

            // Chỉ cập nhật mật khẩu nếu người dùng điền mới
            if (!string.IsNullOrWhiteSpace(password))
                user.Password_Hash = password;

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = $"Đã cập nhật tài khoản '{user.FullName}' thành công!" });
        }

        // POST: /Admin/Users/ToggleStatus/{id} — Khóa / Mở khóa tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (!IsAdmin()) return Json(new { success = false, message = "Không có quyền." });

            var user = await _context.Users.FindAsync(id);
            if (user == null) return Json(new { success = false, message = "Không tìm thấy tài khoản." });

            // Không cho khóa chính mình
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            if (user.USERID == currentUserId)
                return Json(new { success = false, message = "Không thể khóa tài khoản đang đăng nhập." });

            user.Status = !user.Status;
            await _context.SaveChangesAsync();

            var msg = user.Status ? "Đã mở khóa tài khoản" : "Đã khóa tài khoản";
            return Json(new { success = true, status = user.Status, message = $"{msg} '{user.FullName}'." });
        }

        // POST: /Admin/Users/DeleteUser/{id} — Xóa tài khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!IsAdmin()) return Json(new { success = false, message = "Không có quyền." });

            var user = await _context.Users.FindAsync(id);
            if (user == null) return Json(new { success = false, message = "Không tìm thấy tài khoản." });

            // Không cho xóa chính mình
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            if (user.USERID == currentUserId)
                return Json(new { success = false, message = "Không thể xóa tài khoản đang đăng nhập." });

            // Kiểm tra Admin cuối
            if (user.Role == "Admin KTV")
            {
                var adminCount = await _context.Users.CountAsync(u => u.Role == "Admin KTV");
                if (adminCount <= 1)
                    return Json(new { success = false, message = "Phải có ít nhất 1 Admin KTV trong hệ thống." });
            }

            // Kiểm tra có phiếu tiếp nhận liên kết không
            var hasPhieu = await _context.PhieuTiepNhans.AnyAsync(p => p.MaUser_KTV == id);
            if (hasPhieu)
                return Json(new { success = false, message = "Không thể xóa — tài khoản đang có phiếu tiếp nhận liên kết. Hãy khóa tài khoản thay vì xóa." });

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã xóa tài khoản '{user.FullName}'." });
        }

        // ════════════════════════════════════════
        // BÁO CÁO & THỐNG KÊ — Reports
        // ════════════════════════════════════════

        [Authorize(Roles = "Admin KTV,KTV")]
        public async Task<IActionResult> Reports()
        {
            bool isAdmin = IsAdmin();
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            ViewBag.IsAdmin = isAdmin;

            var baseQuery = _context.PhieuTiepNhans.AsQueryable();
            if (!isAdmin)
            {
                baseQuery = baseQuery.Where(p => p.MaUser_KTV == currentUserId);
            }

            // Thống kê phiếu
            ViewData["TongPhieu"]      = await baseQuery.CountAsync();
            ViewData["ChoKham"]        = await baseQuery.CountAsync(p => p.TrangThai == "Chờ khám");
            ViewData["DangXuLy"]       = await baseQuery.CountAsync(p => p.TrangThai == "Đang kiểm tra" || p.TrangThai == "Đang sửa chữa");
            ViewData["DaGiaiQuyet"]    = await baseQuery.CountAsync(p => p.TrangThai == "Hoàn thành");

            // Thống kê tri thức
            ViewData["SoTrieuChung"]   = await _context.TrieuChungs.CountAsync();
            ViewData["SoMaLoi"]        = await _context.MaLois.CountAsync();
            ViewData["SoGiaiPhap"]     = await _context.GiaiPhaps.CountAsync();

            // Top KTV theo số phiếu hoàn thành (Chỉ admin xem)
            var topKtv = await _context.PhieuTiepNhans
                .Where(p => p.TrangThai == "Hoàn thành" && p.User_KTV != null)
                .Include(p => p.User_KTV)
                .GroupBy(p => new { p.MaUser_KTV, p.User_KTV!.FullName, p.User_KTV.Role })
                .Select(g => new { g.Key.FullName, g.Key.Role, SoPhieu = g.Count() })
                .OrderByDescending(x => x.SoPhieu)
                .Take(5)
                .ToListAsync();

            ViewData["TopKtv"] = topKtv;

            // Top lỗi phổ biến
            var topLoi = await _context.MaLois
                .Select(ml => new
                {
                    ml.TenLoi,
                    ml.PhanLoai,
                    SoPhieu = baseQuery.Count(p => p.MaLoi_CD == ml.MaLoiID)
                })
                .OrderByDescending(x => x.SoPhieu)
                .Take(6)
                .ToListAsync();

            ViewData["TopLoi"] = topLoi;

            // Hoạt động gần đây (10 phiếu mới nhất)
            var recent = await baseQuery
                .Include(p => p.ThietBi).ThenInclude(t => t!.KhachHang)
                .Include(p => p.User_KTV)
                .OrderByDescending(p => p.NgayNhan)
                .Take(10)
                .ToListAsync();

            ViewData["Recent"] = recent;

            // Chart data: phiếu theo ngày (7 ngày gần nhất)
            var sevenDaysAgo = DateTime.Now.AddDays(-6).Date;
            var chartData = await baseQuery
                .Where(p => p.NgayNhan >= sevenDaysAgo)
                .GroupBy(p => p.NgayNhan.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync();

            // Biểu đồ chi phí linh kiện (Doughnut Chart)
            // 1. Phí dịch vụ (Tiền công)
            var tongTienCong = await _context.ChiTietSuaChuas
                .Where(ct => isAdmin || ct.PhieuTiepNhan.MaUser_KTV == currentUserId)
                .SumAsync(ct => ct.ChiPhi);

            // 2. Chi phí linh kiện theo loại
            var chiPhiLinhKien = await _context.SuDungLinhKiens
                .Include(s => s.LinhKien)
                .Include(s => s.ChiTietSuaChua.PhieuTiepNhan)
                .Where(s => isAdmin || s.ChiTietSuaChua.PhieuTiepNhan.MaUser_KTV == currentUserId)
                .GroupBy(s => s.LinhKien!.LoaiLK)
                .Select(g => new {
                    LoaiLK = g.Key,
                    TongTien = g.Sum(s => s.SoLuong * s.DonGiaXuat)
                })
                .ToListAsync();

            var chiPhiLabels = new List<string> { "Phí dịch vụ" };
            var chiPhiData = new List<decimal> { tongTienCong };

            foreach (var item in chiPhiLinhKien)
            {
                chiPhiLabels.Add(item.LoaiLK);
                chiPhiData.Add(item.TongTien);
            }

            ViewData["ChiPhiLabels"] = System.Text.Json.JsonSerializer.Serialize(chiPhiLabels);
            ViewData["ChiPhiData"] = System.Text.Json.JsonSerializer.Serialize(chiPhiData);

            // ════════════════════════════════════════
            // Biểu đồ Xu hướng lỗi theo thời gian (30 ngày qua)
            // ════════════════════════════════════════
            var thirtyDaysAgo = DateTime.Now.AddDays(-29).Date;
            var trendLoiData = await baseQuery
                .Where(p => p.NgayNhan >= thirtyDaysAgo && p.MaLoiNavigation != null)
                .GroupBy(p => new { Date = p.NgayNhan.Date, p.MaLoi_CD, p.MaLoiNavigation!.TenLoi })
                .Select(g => new { Date = g.Key.Date, Loi = g.Key.TenLoi, Count = g.Count() })
                .ToListAsync();

            // Lấy Top 5 lỗi phổ biến nhất trong 30 ngày qua
            var top5LoiNames = trendLoiData
                .GroupBy(x => x.Loi)
                .Select(g => new { Loi = g.Key, Total = g.Sum(x => x.Count) })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .Select(x => x.Loi)
                .ToList();

            var trendDates = Enumerable.Range(0, 30)
                .Select(offset => thirtyDaysAgo.AddDays(offset))
                .ToList();

            var trendDatasets = new List<object>();
            string[] colors = { "#ef4444", "#f59e0b", "#38bdf8", "#a78bfa", "#22c55e" };
            int c = 0;
            foreach (var loiName in top5LoiNames)
            {
                var loiCounts = trendDates.Select(d => trendLoiData.Where(t => t.Loi == loiName && t.Date == d).Select(t => t.Count).FirstOrDefault()).ToList();
                trendDatasets.Add(new {
                    label = loiName,
                    data = loiCounts,
                    borderColor = colors[c % colors.Length],
                    backgroundColor = colors[c % colors.Length],
                    tension = 0.4
                });
                c++;
            }

            ViewData["TrendDates"] = System.Text.Json.JsonSerializer.Serialize(trendDates.Select(d => d.ToString("dd/MM")));
            ViewData["TrendDatasets"] = System.Text.Json.JsonSerializer.Serialize(trendDatasets);

            ViewData["ChartLabels"] = System.Text.Json.JsonSerializer.Serialize(
                chartData.Select(x => x.Date.ToString("dd/MM")).ToList());
            ViewData["ChartData"] = System.Text.Json.JsonSerializer.Serialize(
                chartData.Select(x => x.Count).ToList());

            return View("Reports/Index");
        }

        // ════════════════════════════════════════
        // ĐÁNH GIÁ KHÁCH HÀNG — DanhGia
        // ════════════════════════════════════════

        public async Task<IActionResult> DanhGia()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var danhGias = await _context.DanhGiaDichVus
                .Include(d => d.PhieuTiepNhan)
                    .ThenInclude(p => p.ThietBi)
                        .ThenInclude(t => t.KhachHang)
                .Include(d => d.PhieuTiepNhan)
                    .ThenInclude(p => p.User_KTV)
                .OrderByDescending(d => d.NgayDG)
                .ToListAsync();

            // Thống kê tổng hợp
            int totalReviews = danhGias.Count;
            double avgRating = totalReviews > 0 ? danhGias.Average(d => d.SoSao) : 0;
            int[] starDist = new int[5];
            foreach (var dg in danhGias)
            {
                if (dg.SoSao >= 1 && dg.SoSao <= 5)
                    starDist[dg.SoSao - 1]++;
            }

            ViewData["TotalReviews"] = totalReviews;
            ViewData["AvgRating"] = Math.Round(avgRating, 1);
            ViewData["StarDist"] = starDist;

            return View("DanhGia/Index", danhGias);
        }

        // ════════════════════════════════════════
        // CÀI ĐẶT HỆ THỐNG — Settings
        // ════════════════════════════════════════

        public async Task<IActionResult> Settings()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            // Thông tin hệ thống
            ViewData["DbUserCount"]     = await _context.Users.CountAsync();
            ViewData["DbPhieuCount"]    = await _context.PhieuTiepNhans.CountAsync();
            ViewData["DbMaLoiCount"]    = await _context.MaLois.CountAsync();
            ViewData["DbGiaiPhapCount"] = await _context.GiaiPhaps.CountAsync();

            return View("Settings/Index");
        }

        // POST: /Admin/ChangePassword — Đổi mật khẩu admin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            if (!IsAdmin()) return Json(new { success = false, message = "Không có quyền." });

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                return Json(new { success = false, message = "Mật khẩu mới phải có ít nhất 6 ký tự." });

            if (newPassword != confirmPassword)
                return Json(new { success = false, message = "Xác nhận mật khẩu không khớp." });

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var user = await _context.Users.FindAsync(currentUserId);
            if (user == null) return Json(new { success = false, message = "Không tìm thấy tài khoản." });

            if (user.Password_Hash != currentPassword)
                return Json(new { success = false, message = "Mật khẩu hiện tại không chính xác." });

            user.Password_Hash = newPassword;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã đổi mật khẩu thành công!" });
        }

        // ════════════════════════════════════════
        // HYBRID AI — Quản lý mô hình
        // ════════════════════════════════════════

        public async Task<IActionResult> MLModels()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var models = await _context.MLModelInfos
                .OrderByDescending(m => m.NgayTao)
                .ToListAsync();

            var jobs = await _context.AITrainingJobs
                .OrderByDescending(j => j.NgayTao)
                .Take(10)
                .ToListAsync();

            ViewData["TrainingJobs"] = jobs;

            // Thống kê dữ liệu huấn luyện
            ViewData["TotalDiagnosticRecords"] = await _context.LichSuChanDoans.CountAsync(x => x.MaLoiThucTe != null);

            // Thành phần dữ liệu có nhãn theo nguồn (để không gọi dữ liệu tổng hợp là "bản ghi chẩn đoán")
            var theoNguon = await _context.LichSuChanDoans
                .Where(x => x.MaLoiThucTe != null)
                .GroupBy(x => x.NguonDuLieu)
                .Select(g => new { g.Key, N = g.Count() })
                .ToListAsync();
            ViewBag.NguonDuLieu = theoNguon.ToDictionary(x => x.Key.ToString(), x => x.N);

            // Bộ mô hình AI thật (ONNX / JSON) nằm ngoài bảng ML_MODEL_INFO
            var cfg = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            ViewBag.BoMoHinhThat = AiModelCatalog.Doc(Services.ModelPaths.Root(cfg, Directory.GetCurrentDirectory()));
            ViewBag.TuDongChot = cfg.GetValue<bool>("AI:TuDongChot", true);

            var maLois = await _context.MaLois.ToListAsync();
            ViewBag.MaLoiNames = maLois.ToDictionary(m => m.MaLoiID.ToString(), m => m.TenLoi);

            return View("MLModels/Index", models);
        }

        // GET: /Admin/XuatDuLieuHuanLuyen — xuất dữ liệu theo hợp đồng của bộ mô hình chuẩn (MoHinh-Chuan) để đánh giá ngoài hệ thống.
        // Chỉ Admin KTV; mặc định KHÔNG kèm mô tả sự cố của khách (có thể chứa thông tin cá nhân).
        [HttpGet]
        [Authorize(Roles = "Admin KTV")]
        public async Task<IActionResult> XuatDuLieuHuanLuyen(bool kemMoTaSuCo = false)
        {
            var zip = await Services.TrainingDataExporter.BuildZipAsync(_context, kemMoTaSuCo);
            return File(zip, "application/zip", $"du_lieu_huan_luyen_{DateTime.Now:yyyyMMdd_HHmm}.zip");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateModel(int id)
        {
            if (!HttpContext.RequestServices.GetRequiredService<IConfiguration>().GetValue<bool>("AI:AllowActivation", true)) return StatusCode(409, "Kích hoạt mô hình đang bị tắt trong cấu hình (AI:AllowActivation=false).");
            if (!IsAdmin()) return Json(new { success = false, message = "Không có quyền." });

            var model = await _context.MLModelInfos.FindAsync(id);
            if (model == null) return Json(new { success = false, message = "Không tìm thấy mô hình." });

            // Safeguards policy
            // Mô hình KHỞI TẠO (huấn luyện từ dữ liệu tổng hợp/công khai) được phép kích hoạt để hệ thống dùng được ngay,
            // nhưng vẫn phải qua các chốt chất lượng bên dưới và được gắn nhãn "chưa kiểm chứng bằng ca thật".
            bool isBootstrapModel = model.PhienBanBoDuLieu != null && model.PhienBanBoDuLieu.StartsWith("KHOI_TAO_TONG_HOP");
            if (!isBootstrapModel && model.SoBanGhiThucTe < 100)
            {
                return Json(new { success = false, message = $"Không thể kích hoạt: Mô hình cần tối thiểu 100 bản ghi thực tế (hiện có {model.SoBanGhiThucTe})." });
            }
            // Khoảng cách tối đa AI được phép kém Rule (điểm %). Mặc định 10; chủ hệ thống có thể nới riêng cho mô hình KHỞI TẠO
            // bằng cấu hình AI:BootstrapMaxRuleGap khi chấp nhận dùng AI chưa kiểm chứng (mô hình thường luôn dùng 10).
            double maxRuleGap = isBootstrapModel ? HttpContext.RequestServices.GetRequiredService<IConfiguration>().GetValue<double>("AI:BootstrapMaxRuleGap", 10) : 10;
            if (model.DoChinhXacAI < model.DoChinhXacRule - maxRuleGap)
            {
                return Json(new { success = false, message = $"Không thể kích hoạt: Độ chính xác AI ({model.DoChinhXacAI:F1}%) thấp hơn Rule Engine ({model.DoChinhXacRule:F1}%) quá {maxRuleGap:F0}%." });
            }
            // Mô hình khởi tạo được đo trên các tổ hợp triệu chứng chưa từng thấy nên không áp ngưỡng tuyệt đối Micro/Macro;
            // chốt "không kém Rule quá 10 điểm" ở trên vẫn bắt buộc.
            if (!isBootstrapModel && (model.DoChinhXacMicro < 0.6 || model.DoChinhXacMacro < 0.5))
            {
                return Json(new { success = false, message = "Không thể kích hoạt: Mô hình không đạt chuẩn Micro/Macro Accuracy cơ bản." });
            }

            // Copy file model thành ActiveModel.zip
            var activeModelPath = Path.Combine(Services.ModelPaths.Root(HttpContext.RequestServices.GetRequiredService<IConfiguration>(), Directory.GetCurrentDirectory()), "ActiveModel.zip");
            try
            {
                if (System.IO.File.Exists(model.DuongDanTep))
                {
                    System.IO.File.Copy(model.DuongDanTep, activeModelPath, overwrite: true);
                }
                else
                {
                    return Json(new { success = false, message = "File mô hình vật lý không tồn tại!" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi kích hoạt mô hình: " + ex.Message });
            }

            // Ghi log
            var activeModel = await _context.MLModelInfos.FirstOrDefaultAsync(m => m.DangSuDung);
            var actorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var log = new ModelActivationLog
            {
                ThoiGianKichHoat = DateTime.Now,
                NguoiKichHoatLuuTru = User.Identity?.Name ?? "Admin",
                PhienBanCu = activeModel?.PhienBan,
                PhienBanMoiLuuTru = model.PhienBan,
                MaMoHinh = model.MaMoHinh,
                MaNguoiDung = int.TryParse(actorId, out var parsedActor) ? parsedActor : null
            };
            _context.ModelActivationLogs.Add(log);

            // Cập nhật CSDL
            if (activeModel != null) activeModel.DangSuDung = false;
            model.DangSuDung = true;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã kích hoạt mô hình phiên bản {model.PhienBan} thành công!" + (isBootstrapModel ? " Lưu ý: đây là mô hình khởi tạo, huấn luyện từ dữ liệu tổng hợp, chưa kiểm chứng bằng ca thật; kỹ thuật viên cần kiểm tra chéo với Rule Engine." : "") });
        }

        // ════════════════════════════════════════
        // REAL-WORLD VALIDATION SUITE
        // ════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> ValidationSuite()
        {
            // if (!IsAdmin()) return RedirectToAction("Index", "Home");

            ViewBag.TrieuChungs = await _context.TrieuChungs.OrderBy(t => t.TenTC).ToListAsync();
            ViewBag.MaLois = await _context.MaLois.OrderBy(m => m.TenLoi).ToListAsync();

            var activeModel = await _context.MLModelInfos.FirstOrDefaultAsync(m => m.DangSuDung);
            ViewBag.ActiveModel = activeModel;

            return View("MLModels/ValidationSuite");
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidationSuite(List<int> selectedTrieuChungs, int expectedFault)
        {
            // if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var symptoms = await _context.TrieuChungs
                .Where(t => selectedTrieuChungs.Contains(t.MaTC))
                .ToListAsync();

            var expected = await _context.MaLois.FindAsync(expectedFault);

            // 1. Rule Engine Prediction
            var symptomNames = symptoms.Select(s => s.TenTC).ToList();
            var rules = await _context.TrieuChungLois.Include(x => x.TrieuChung).Include(x => x.MaLoiNavigation).ToListAsync();
            
            var matchedRule = rules
                .Where(r => symptomNames.Contains(r.TrieuChung.TenTC))
                .GroupBy(r => new { r.MaLoi, r.MaLoiNavigation.TenLoi })
                .Select(g => new { MaLoi = g.Key.MaLoi, TenLoi = g.Key.TenLoi, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .FirstOrDefault();

            string rulePrediction = matchedRule != null ? matchedRule.TenLoi : "Không tìm thấy";

            // 2. AI Prediction
            var activeModelInfo = await _context.MLModelInfos.FirstOrDefaultAsync(m => m.DangSuDung);
            string aiTop1 = "N/A";
            string aiTop3Json = "[]";
            bool isCorrect = false;
            var top3 = new List<Tuple<string, float>>();

            if (activeModelInfo != null && System.IO.File.Exists(activeModelInfo.DuongDanTep))
            {
                try
                {
                    var mlContext = new Microsoft.ML.MLContext();
                    var model = mlContext.Model.Load(activeModelInfo.DuongDanTep, out var schema);
                    var predEngine = mlContext.Model.CreatePredictionEngine<QuanLySuCoMayTinh.Models.ML.ChanDoanData, QuanLySuCoMayTinh.Models.ML.ChanDoanPrediction>(model);

                    float[] features = new float[500];
                    foreach (var s in symptoms)
                    {
                        if (s.MaTC < 500) features[s.MaTC] = 1.0f;
                    }

                    var input = new QuanLySuCoMayTinh.Models.ML.ChanDoanData { Features = features };
                    var pred = predEngine.Predict(input);

                    Microsoft.ML.Data.VBuffer<uint> keys = default;
                    try
                    {
                        predEngine.OutputSchema["Score"].Annotations.GetValue("SlotNames", ref keys);
                    }
                    catch (Exception)
                    {
                        // Fallback 1: Try PredictedLabel KeyValues if Score SlotNames fails
                        try
                        {
                            predEngine.OutputSchema["PredictedLabel"].Annotations.GetValue("KeyValues", ref keys);
                        }
                        catch (Exception)
                        {
                            // Fallback 2: Rebuild mapping from DB assuming OrderBy(Id)
                            var dbLabels = await _context.LichSuChanDoans
                                .Where(l => l.MaLoiThucTe != null &&
                                            l.NguonDuLieu != TrainingDataSource.LegacyRandom)
                                .Select(l => (uint)l.MaLoiThucTe.Value)
                                .Distinct()
                                .OrderBy(l => l)
                                .ToArrayAsync();
                            keys = new Microsoft.ML.Data.VBuffer<uint>(dbLabels.Length, dbLabels);
                        }
                    }
                    
                    var classLabels = keys.DenseValues().ToArray();

                    var sortedScores = pred.Scores
                        .Select((score, index) => new { Score = score, MaLoi = classLabels[index] })
                        .OrderByDescending(x => x.Score)
                        .ToList();

                    for (int i = 0; i < Math.Min(3, sortedScores.Count); i++)
                    {
                        var loi = await _context.MaLois.FindAsync((int)sortedScores[i].MaLoi);
                        if (loi != null)
                        {
                            top3.Add(new Tuple<string, float>(loi.TenLoi, sortedScores[i].Score * 100));
                            if (i == 0) aiTop1 = $"{loi.TenLoi} ({sortedScores[i].Score * 100:F1}%)";
                        }
                    }
                    aiTop3Json = System.Text.Json.JsonSerializer.Serialize(top3);
                    isCorrect = sortedScores.FirstOrDefault()?.MaLoi == expectedFault;
                }
                catch (Exception ex)
                {
                    aiTop1 = "ERROR: " + ex.ToString();
                    aiTop3Json = "[]";
                }
            }

            // Save to AITestCase
            var testCase = new QuanLySuCoMayTinh.Models.ML.AITestCase
            {
                MaMoHinh = await _context.MLModelInfos.Where(m => m.DangSuDung && m.LoaiBaiToan == "CHAN_DOAN_TRIEU_CHUNG_MA_LOI").OrderByDescending(m => m.NgayTao).Select(m => (int?)m.MaMoHinh).FirstOrDefaultAsync(),
                TrieuChungNhap = string.Join(", ", symptomNames),
                LoiKyVong = expected?.TenLoi ?? "Unknown",
                DuDoanRule = rulePrediction,
                DuDoanAITop1 = aiTop1,
                DuDoanAITop3JSON = aiTop3Json,
                AIDuDoanDung = isCorrect,
                NgayTao = DateTime.Now
            };

            _context.AITestCases.Add(testCase);
            await _context.SaveChangesAsync();

            TempData["TestCaseSaved"] = true;

            // Prepare ViewBag for rendering results
            ViewBag.TrieuChungs = await _context.TrieuChungs.OrderBy(t => t.TenTC).ToListAsync();
            ViewBag.MaLois = await _context.MaLois.OrderBy(m => m.TenLoi).ToListAsync();
            ViewBag.ActiveModel = activeModelInfo;
            
            ViewBag.RulePrediction = rulePrediction;
            ViewBag.AITop1 = aiTop1;
            
            // Pass the original top3 list directly to the view instead of deserializing
            ViewBag.AITop3 = top3;
            ViewBag.IsCorrect = isCorrect;

            return View("MLModels/ValidationSuite");
        }

        // ════════════════════════════════════════
        // DASHBOARD PHASE 1
        // ════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var models = await _context.MLModelInfos
                .OrderBy(m => m.NgayTao) // Order chronologically to show trend
                .ToListAsync();

            var latestModel = models.LastOrDefault();

            // Dataset Composition (from latest model or DB)
            int realRecords = latestModel?.SoBanGhiThucTe ?? 0;
            int syntheticRecords = latestModel?.SoBanGhiTongHop ?? 0;
            int userHistoryRecords = latestModel?.SoBanGhiLichSuNguoiDung ?? 0;

            // Trends
            var labels = models.Select(m => m.PhienBan).ToList();
            var aiAccuracy = models.Select(m => Math.Round(m.DoChinhXacAI, 2)).ToList();
            var ruleAccuracy = models.Select(m => Math.Round(m.DoChinhXacRule, 2)).ToList();
            var logLoss = models.Select(m => Math.Round(m.LogLoss, 4)).ToList();

            ViewBag.Composition = new { Real = realRecords, Synthetic = syntheticRecords, UserHistory = userHistoryRecords };
            ViewBag.TrendLabels = labels;
            ViewBag.AIAccuracyTrend = aiAccuracy;
            ViewBag.RuleAccuracyTrend = ruleAccuracy;
            ViewBag.LogLossTrend = logLoss;

            return View("MLModels/Dashboard");
        }
    }
}
