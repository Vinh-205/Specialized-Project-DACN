using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Data;
using QuanLySuCoMayTinh.Models;

namespace QuanLySuCoMayTinh.Controllers
{
    [Authorize]
    public class SuDungLinhKienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuDungLinhKienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /SuDungLinhKien/XuatKho
        // ─────────────────────────────────────────────────────────────────────────
        // Luồng nghiệp vụ (toàn bộ trong 1 IDbContextTransaction):
        //  1. Lock record LinhKien để đọc tồn kho mới nhất (tránh race condition).
        //  2. Kiểm tra SoLuongTon >= SoLuongYeuCau.
        //  3. Tạo ChiTietSuaChua mới với TenHangMuc = tên linh kiện và ChiPhi = tổng tiền.
        //  4. Tạo SuDungLinhKien ghi lại DonGiaXuat tại thời điểm xuất.
        //  5. Trừ SoLuongTon trong LinhKien.
        //  6. Commit transaction.
        // ─────────────────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XuatKho(int maPhieu, int maLK, int soLuong, string? ghiChu)
        {
            // ── Validate đầu vào cơ bản ──────────────────────────────────────────
            if (soLuong <= 0)
            {
                TempData["ErrorMsg"] = "Số lượng xuất phải ít nhất là 1.";
                return RedirectToAction("Details", "PhieuTiepNhan", new { id = maPhieu });
            }

            // Khóa cứng: Chặn xuất kho khi phiếu đã Hoàn thành (Bước 5)
            var phieuCheck = await _context.PhieuTiepNhans.FindAsync(maPhieu);
            if (phieuCheck != null && phieuCheck.TrangThai == "Hoàn thành")
            {
                TempData["ErrorMsg"] = "Phiếu đã thanh toán và khóa cứng. Không thể xuất thêm linh kiện.";
                return RedirectToAction("Details", "PhieuTiepNhan", new { id = maPhieu });
            }

            // ── Mở Transaction ────────────────────────────────────────────────────
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // STEP 1: Đọc linh kiện với lock (UPDLOCK tương đương trong EF = reload sau lock)
                // Dùng AsNoTracking = false để EF theo dõi và cập nhật entity
                var linhKien = await _context.LinhKiens
                    .FirstOrDefaultAsync(lk => lk.MaLK == maLK);

                if (linhKien is null)
                {
                    TempData["ErrorMsg"] = "Linh kiện không tồn tại trong kho.";
                    await transaction.RollbackAsync();
                    return RedirectToAction("Details", "PhieuTiepNhan", new { id = maPhieu });
                }

                if (!linhKien.CoTrangThai)
                {
                    TempData["ErrorMsg"] = $"Linh kiện \"{linhKien.TenLK}\" đã ngừng kinh doanh, không thể xuất kho.";
                    await transaction.RollbackAsync();
                    return RedirectToAction("Details", "PhieuTiepNhan", new { id = maPhieu });
                }

                // STEP 2: Kiểm tra tồn kho đủ không
                if (linhKien.SoLuongTon < soLuong)
                {
                    TempData["ErrorMsg"] = $"Tồn kho không đủ! \"{linhKien.TenLK}\" chỉ còn {linhKien.SoLuongTon} cái trong kho.";
                    await transaction.RollbackAsync();
                    return RedirectToAction("Details", "PhieuTiepNhan", new { id = maPhieu });
                }

                // Chốt giá xuất tại thời điểm này (bất biến, không phụ thuộc giá kho tương lai)
                var donGiaXuat = linhKien.DonGia;
                var tongTien   = donGiaXuat * soLuong;
                var tenHangMuc = soLuong > 1
                    ? $"{linhKien.TenLK} x{soLuong}"
                    : linhKien.TenLK;

                // STEP 3: Tạo ChiTietSuaChua mới
                var chiTiet = new ChiTietSuaChua
                {
                    MaPhieu    = maPhieu,
                    TenHangMuc = tenHangMuc,
                    ChiPhi     = tongTien,
                    GhiChu     = string.IsNullOrWhiteSpace(ghiChu) ? null : ghiChu.Trim()
                };
                _context.ChiTietSuaChuas.Add(chiTiet);

                // SaveChanges lần 1 để EF sinh ra MaCT (identity key) cần cho FK bên dưới
                await _context.SaveChangesAsync();

                // STEP 4: Tạo SuDungLinhKien với Composite Key {MaCT, MaLK}
                var suDung = new SuDungLinhKien
                {
                    MaCT        = chiTiet.MaCT,   // MaCT đã được DB cấp sau SaveChanges
                    MaLK        = maLK,
                    SoLuong     = soLuong,
                    DonGiaXuat  = donGiaXuat       // Chốt giá tại thời điểm xuất
                };
                _context.SuDungLinhKiens.Add(suDung);

                // STEP 5: Trừ lùi số lượng tồn kho
                linhKien.SoLuongTon -= soLuong;

                // SaveChanges lần 2 — ghi SuDungLinhKien + cập nhật LinhKien
                await _context.SaveChangesAsync();

                // STEP 6: Commit toàn bộ transaction
                await transaction.CommitAsync();

                TempData["SuccessMsg"] = $"Đã xuất {soLuong} cái \"{linhKien.TenLK}\" ({string.Format("{0:N0}", tongTien)} đ) vào phiếu. Tồn kho còn lại: {linhKien.SoLuongTon}.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMsg"] = $"Lỗi hệ thống khi xuất kho: {ex.Message}";
            }

            return RedirectToAction("Details", "PhieuTiepNhan", new { id = maPhieu });
        }

        // POST: /SuDungLinhKien/HoanKho — Hoàn trả linh kiện (xóa dòng chi tiết + cộng lại kho)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin KTV,Admin")]
        public async Task<IActionResult> HoanKho(int maCT, int maLK, int maPhieu)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var suDung = await _context.SuDungLinhKiens
                    .Include(s => s.ChiTietSuaChua)
                    .Include(s => s.LinhKien)
                    .FirstOrDefaultAsync(s => s.MaCT == maCT && s.MaLK == maLK);

                if (suDung is null)
                {
                    TempData["ErrorMsg"] = "Không tìm thấy bản ghi sử dụng linh kiện.";
                    await transaction.RollbackAsync();
                    return RedirectToAction("Details", "PhieuTiepNhan", new { id = maPhieu });
                }

                // Hoàn trả số lượng về kho
                if (suDung.LinhKien is not null)
                    suDung.LinhKien.SoLuongTon += suDung.SoLuong;

                // Xóa dòng SuDungLinhKien trước (do FK constraint)
                _context.SuDungLinhKiens.Remove(suDung);

                // Xóa luôn dòng ChiTietSuaChua tương ứng
                if (suDung.ChiTietSuaChua is not null)
                    _context.ChiTietSuaChuas.Remove(suDung.ChiTietSuaChua);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMsg"] = "Đã hoàn trả linh kiện về kho thành công.";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMsg"] = $"Lỗi khi hoàn kho: {ex.Message}";
            }

            return RedirectToAction("Details", "PhieuTiepNhan", new { id = maPhieu });
        }
    }
}
