using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Data;
using QuanLySuCoMayTinh.Models;

namespace QuanLySuCoMayTinh.Controllers
{
    [Authorize]
    public class LinhKienController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LinhKienController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /LinhKien — Danh sách kho linh kiện
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.LinhKiens
                .OrderBy(lk => lk.LoaiLK)
                .ThenBy(lk => lk.TenLK)
                .ToListAsync();

            return View(danhSach);
        }

        // GET: /LinhKien/Create
        [Authorize(Roles = "Admin KTV,Admin")]
        public IActionResult Create()
        {
            return View(new LinhKien { CoTrangThai = true });
        }

        // POST: /LinhKien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin KTV,Admin")]
        public async Task<IActionResult> Create(LinhKien model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.TenLK  = model.TenLK.Trim();
            model.LoaiLK = model.LoaiLK.Trim();

            _context.LinhKiens.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMsg"] = $"Đã thêm linh kiện \"{model.TenLK}\" vào kho.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /LinhKien/Edit/{id}
        [Authorize(Roles = "Admin KTV,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var linhKien = await _context.LinhKiens.FindAsync(id);
            if (linhKien is null) return NotFound();
            return View(linhKien);
        }

        // POST: /LinhKien/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin KTV,Admin")]
        public async Task<IActionResult> Edit(int id, LinhKien model)
        {
            if (id != model.MaLK) return BadRequest();
            if (!ModelState.IsValid)  return View(model);

            try
            {
                model.TenLK  = model.TenLK.Trim();
                model.LoaiLK = model.LoaiLK.Trim();
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["SuccessMsg"] = "Đã cập nhật thông tin linh kiện.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.LinhKiens.AnyAsync(lk => lk.MaLK == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /LinhKien/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin KTV,Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var linhKien = await _context.LinhKiens
                .Include(lk => lk.SuDungLinhKiens)
                .FirstOrDefaultAsync(lk => lk.MaLK == id);

            if (linhKien is null) return NotFound();

            // Bảo vệ: Không xóa linh kiện đã từng được sử dụng trong phiếu
            if (linhKien.SuDungLinhKiens.Any())
            {
                TempData["ErrorMsg"] = $"Không thể xóa \"{linhKien.TenLK}\" vì đã được sử dụng trong {linhKien.SuDungLinhKiens.Count} phiếu sửa chữa.";
                return RedirectToAction(nameof(Index));
            }

            _context.LinhKiens.Remove(linhKien);
            await _context.SaveChangesAsync();
            TempData["SuccessMsg"] = $"Đã xóa linh kiện \"{linhKien.TenLK}\" khỏi kho.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /LinhKien/NhapKho — Nhập thêm hàng vào kho (tăng SoLuongTon)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin KTV,Admin")]
        public async Task<IActionResult> NhapKho(int id, int soLuongNhap)
        {
            if (soLuongNhap <= 0)
            {
                TempData["ErrorMsg"] = "Số lượng nhập phải lớn hơn 0.";
                return RedirectToAction(nameof(Index));
            }

            var linhKien = await _context.LinhKiens.FindAsync(id);
            if (linhKien is null) return NotFound();

            linhKien.SoLuongTon += soLuongNhap;
            await _context.SaveChangesAsync();

            TempData["SuccessMsg"] = $"Đã nhập thêm {soLuongNhap} cái \"{linhKien.TenLK}\". Tồn kho hiện tại: {linhKien.SoLuongTon}.";
            return RedirectToAction(nameof(Index));
        }

            // GET: /LinhKien/FixTonKhoLichSu
            // CÔNG CỤ ẨN: Chạy URL này trên trình duyệt ĐÚNG 1 LẦN DUY NHẤT để 
            // cập nhật lại chính xác số lượng tiêu hao từ dữ liệu cũ
            [HttpGet]
            [Authorize(Roles = "Admin KTV,Admin")]
            public async Task<IActionResult> FixTonKhoLichSu()
            {
                var daXuat = await _context.SuDungLinhKiens
                    .GroupBy(s => s.MaLK)
                    .Select(g => new { MaLK = g.Key, TongSuDung = g.Sum(x => x.SoLuong) })
                    .ToListAsync();

                foreach (var item in daXuat)
                {
                    var lk = await _context.LinhKiens.FindAsync(item.MaLK);
                    if (lk != null)
                    {
                        lk.SoLuongTon -= item.TongSuDung;
                        if (lk.SoLuongTon < 0) lk.SoLuongTon = 0;
                    }
                }
                await _context.SaveChangesAsync();

                TempData["SuccessMsg"] = "✅ Đã đồng bộ và cập nhật chính xác số lượng linh kiện tiêu hao từ trước đến giờ!";
                return RedirectToAction(nameof(Index));
            }
    }
}
