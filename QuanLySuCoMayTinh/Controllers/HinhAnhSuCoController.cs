using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Data;
using QuanLySuCoMayTinh.Models;

namespace QuanLySuCoMayTinh.Controllers
{
    [Authorize]
    public class HinhAnhSuCoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        // Các định dạng ảnh được phép
        private static readonly HashSet<string> _allowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public HinhAnhSuCoController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // POST: /HinhAnhSuCo/Upload
        // Nhận file ảnh, lưu vào wwwroot/uploads/suco/{MaPhieu}/, ghi metadata vào DB.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int MaPhieu, IFormFile AnhFile, string? MoTa)
        {
            // ── 1. Validate phiếu tồn tại ──────────────────────────────────────
            var phieu = await _context.PhieuTiepNhans.FindAsync(MaPhieu);
            if (phieu is null)
            {
                TempData["ErrorMsg"] = "Phiếu tiếp nhận không tồn tại.";
                return RedirectToAction("Index", "PhieuTiepNhan");
            }

            // ── 2. Validate file ────────────────────────────────────────────────
            if (AnhFile is null || AnhFile.Length == 0)
            {
                TempData["ErrorMsg"] = "Vui lòng chọn file ảnh trước khi tải lên.";
                return RedirectToAction("Details", "PhieuTiepNhan", new { id = MaPhieu });
            }

            if (AnhFile.Length > MaxFileSizeBytes)
            {
                TempData["ErrorMsg"] = "Kích thước ảnh vượt quá 5MB. Vui lòng chọn ảnh nhỏ hơn.";
                return RedirectToAction("Details", "PhieuTiepNhan", new { id = MaPhieu });
            }

            var ext = Path.GetExtension(AnhFile.FileName);
            if (!_allowedExtensions.Contains(ext))
            {
                TempData["ErrorMsg"] = "Định dạng ảnh không hợp lệ. Chỉ chấp nhận JPG, PNG, GIF, WEBP.";
                return RedirectToAction("Details", "PhieuTiepNhan", new { id = MaPhieu });
            }

            // ── 3. Tạo đường dẫn lưu file ──────────────────────────────────────
            // Cấu trúc thư mục: wwwroot/uploads/suco/{MaPhieu}/{timestamp}_{randomGuid}.ext
            var folderRelative = Path.Combine("uploads", "suco", MaPhieu.ToString());
            var folderAbsolute = Path.Combine(_env.WebRootPath, folderRelative);

            // Tạo thư mục nếu chưa tồn tại
            Directory.CreateDirectory(folderAbsolute);

            // Tên file: timestamp + 6 ký tự ngẫu nhiên để tránh xung đột
            var safeFileName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{Guid.NewGuid().ToString("N")[..6]}{ext}";
            var fileAbsolutePath = Path.Combine(folderAbsolute, safeFileName);
            var fileRelativePath = Path.Combine(folderRelative, safeFileName).Replace('\\', '/');

            // ── 4. Ghi file xuống disk ──────────────────────────────────────────
            await using (var stream = new FileStream(fileAbsolutePath, FileMode.Create))
            {
                await AnhFile.CopyToAsync(stream);
            }

            // ── 5. Ghi metadata vào database ───────────────────────────────────
            var hinhAnh = new HinhAnhSuCo
            {
                MaPhieu    = MaPhieu,
                DuongDanAnh = fileRelativePath,
                MoTa       = string.IsNullOrWhiteSpace(MoTa) ? null : MoTa.Trim(),
                NgayChup   = DateTime.Now
            };

            _context.HinhAnhSuCos.Add(hinhAnh);
            await _context.SaveChangesAsync();

            TempData["SuccessMsg"] = "Tải ảnh lên thành công!";
            return RedirectToAction("Details", "PhieuTiepNhan", new { id = MaPhieu });
        }

        // POST: /HinhAnhSuCo/Delete/{id}
        // Xóa ảnh khỏi DB và disk. Trả về JSON cho fetch() từ JS.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var hinhAnh = await _context.HinhAnhSuCos.FindAsync(id);
            if (hinhAnh is null)
                return NotFound(new { message = "Không tìm thấy hình ảnh." });

            var maPhieu = hinhAnh.MaPhieu;

            // Xóa file vật lý trên disk
            var fileAbsolutePath = Path.Combine(_env.WebRootPath, hinhAnh.DuongDanAnh.Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(fileAbsolutePath))
            {
                System.IO.File.Delete(fileAbsolutePath);
            }

            // Xóa record trong database
            _context.HinhAnhSuCos.Remove(hinhAnh);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa ảnh thành công.", maPhieu });
        }
        // GET: /HinhAnhSuCo/CountPhotos/{maPhieu}
        // Trả về số lượng ảnh hiện tại — cho polling từ trang Details
        [AllowAnonymous]
        public async Task<IActionResult> CountPhotos(int maPhieu)
        {
            var count = await _context.HinhAnhSuCos
                .Where(h => h.MaPhieu == maPhieu)
                .CountAsync();
            return Json(new { count });
        }

        // GET: /HinhAnhSuCo/MobileUpload/{maPhieu}
        // Trang upload ảnh không cần đăng nhập — dành cho thiết bị di động quét QR
        [AllowAnonymous]
        [Route("HinhAnhSuCo/MobileUpload/{maPhieu:int}")]
        public async Task<IActionResult> MobileUpload(int maPhieu)
        {
            var phieu = await _context.PhieuTiepNhans
                .Include(p => p.ThietBi)
                    .ThenInclude(t => t.KhachHang)
                .FirstOrDefaultAsync(p => p.MaPhieu == maPhieu);

            if (phieu is null) return NotFound();

            ViewBag.MaPhieu = maPhieu;
            ViewBag.TenThietBi = phieu.ThietBi?.TenTB ?? "Thiết bị";
            ViewBag.TenKhachHang = phieu.ThietBi?.KhachHang?.HoTen ?? "Khách hàng";
            return View();
        }

        // POST: /HinhAnhSuCo/MobileUploadPost
        // Nhận ảnh từ thiết bị di động (AllowAnonymous + bỏ kiểm tra AntiForgery vì không có cookie session)
        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> MobileUploadPost(int MaPhieu, IFormFile AnhFile, string? MoTa)
        {
            var phieu = await _context.PhieuTiepNhans.FindAsync(MaPhieu);
            if (phieu is null)
                return Json(new { success = false, message = "Phiếu không tồn tại." });

            if (AnhFile is null || AnhFile.Length == 0)
                return Json(new { success = false, message = "Chưa chọn ảnh." });

            if (AnhFile.Length > MaxFileSizeBytes)
                return Json(new { success = false, message = "Ảnh quá lớn (tối đa 5MB)." });

            var ext = Path.GetExtension(AnhFile.FileName);
            if (!_allowedExtensions.Contains(ext))
                return Json(new { success = false, message = "Định dạng không hợp lệ. Chỉ chấp nhận JPG, PNG, WEBP." });

            var folderRelative = Path.Combine("uploads", "suco", MaPhieu.ToString());
            var folderAbsolute = Path.Combine(_env.WebRootPath, folderRelative);
            Directory.CreateDirectory(folderAbsolute);

            var safeFileName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}_{Guid.NewGuid().ToString("N")[..6]}{ext}";
            var fileAbsolutePath = Path.Combine(folderAbsolute, safeFileName);
            var fileRelativePath = Path.Combine(folderRelative, safeFileName).Replace('\\', '/');

            await using (var stream = new FileStream(fileAbsolutePath, FileMode.Create))
            {
                await AnhFile.CopyToAsync(stream);
            }

            var hinhAnh = new HinhAnhSuCo
            {
                MaPhieu     = MaPhieu,
                DuongDanAnh = fileRelativePath,
                MoTa        = string.IsNullOrWhiteSpace(MoTa) ? "Ảnh từ thiết bị di động" : MoTa.Trim(),
                NgayChup    = DateTime.Now
            };
            _context.HinhAnhSuCos.Add(hinhAnh);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Tải ảnh lên thành công!" });
        }
    }
}
