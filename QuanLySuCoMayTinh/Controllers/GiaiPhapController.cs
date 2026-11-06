using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Data;
using QuanLySuCoMayTinh.Models;

namespace QuanLySuCoMayTinh.Controllers
{
    public class GiaiPhapController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GiaiPhapController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.GiaiPhaps.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(gp => gp.NoiDungStepByStep.Contains(searchString) || gp.MaLoi.ToString().Contains(searchString));
            }
            
            ViewData["CurrentSearch"] = searchString;
            var danhSachGP = await query.ToListAsync();
            return View(danhSachGP);
        }

        public IActionResult Create()
        {
            // Dropdown chọn Mã Lỗi (lấy MaLoiID làm value, TenLoi làm chữ hiển thị)
            ViewData["DanhSachMaLoi"] = new SelectList(_context.MaLois, "MaLoiID", "TenLoi");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Đổi tên biến cho khớp với Model của Vĩnh: MaLoi, NoiDungStepByStep
        public async Task<IActionResult> Create([Bind("MaLoi,NoiDungStepByStep")] GiaiPhap giaiPhap)
        {
            // Bỏ qua kiểm tra property liên kết
            ModelState.Remove("MaLoiNavigation");

            if (ModelState.IsValid)
            {
                _context.Add(giaiPhap);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhSachMaLoi"] = new SelectList(_context.MaLois, "MaLoiID", "TenLoi", giaiPhap.MaLoi);
            return View(giaiPhap);
        }

        // GET: Hiển thị form chỉnh sửa Giải Pháp
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var giaiPhap = await _context.GiaiPhaps.FindAsync(id);
            if (giaiPhap == null) return NotFound();

            // Nạp lại danh sách Mã lỗi để hiển thị lên Dropdown
            ViewData["DanhSachMaLoi"] = new SelectList(_context.MaLois, "MaLoiID", "TenLoi", giaiPhap.MaLoi);
            return View(giaiPhap);
        }

        // POST: Nhận dữ liệu sửa đổi và lưu vào DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaGP,MaLoi,NoiDungStepByStep,MucDoKho,Status")] GiaiPhap giaiPhap)
        {
            if (id != giaiPhap.MaGP) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giaiPhap);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.GiaiPhaps.Any(e => e.MaGP == giaiPhap.MaGP)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DanhSachMaLoi"] = new SelectList(_context.MaLois, "MaLoiID", "TenLoi", giaiPhap.MaLoi);
            return View(giaiPhap);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gp = await _context.GiaiPhaps.FindAsync(id);
            if (gp != null)
            {
                _context.GiaiPhaps.Remove(gp);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}