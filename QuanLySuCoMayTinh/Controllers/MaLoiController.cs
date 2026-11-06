using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Data;
using QuanLySuCoMayTinh.Models;

namespace QuanLySuCoMayTinh.Controllers
{
    public class MaLoiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaLoiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách Mã Lỗi
        public async Task<IActionResult> Index(string searchString, string phanLoai)
        {
            var query = _context.MaLois.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(ml => ml.TenLoi.Contains(searchString));
            }
            
            if (!string.IsNullOrEmpty(phanLoai) && phanLoai != "Tất cả phân loại")
            {
                query = query.Where(ml => ml.PhanLoai == phanLoai);
            }
            
            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentFilter"] = string.IsNullOrEmpty(phanLoai) ? "Tất cả phân loại" : phanLoai;
            return View(await query.ToListAsync());
        }

        // Hiển thị Form Thêm mới
        public IActionResult Create()
        {
            return View();
        }

        // Lưu dữ liệu Thêm mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenLoi,PhanLoai,Status")] MaLoi maLoi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(maLoi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(maLoi);
        }

        // Hiển thị Form Sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var maLoi = await _context.MaLois.FindAsync(id);
            if (maLoi == null) return NotFound();
            return View(maLoi);
        }

        // Lưu dữ liệu Sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLoiID,TenLoi,PhanLoai,Status")] MaLoi maLoi)
        {
            if (id != maLoi.MaLoiID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(maLoi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.MaLois.Any(e => e.MaLoiID == maLoi.MaLoiID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(maLoi);
        }

        // Xóa Mã Lỗi
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var maLoi = await _context.MaLois.FindAsync(id);
            if (maLoi != null)
            {
                _context.MaLois.Remove(maLoi);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}