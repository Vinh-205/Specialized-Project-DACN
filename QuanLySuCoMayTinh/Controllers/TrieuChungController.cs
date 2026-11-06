using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Data;
using QuanLySuCoMayTinh.Models;

namespace QuanLySuCoMayTinh.Controllers
{
    [Authorize]
    public class TrieuChungController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor: Tiêm (Inject) ApplicationDbContext vào Controller
        public TrieuChungController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action Index: Lấy dữ liệu từ bảng TRIEU_CHUNG và đẩy ra View
        public async Task<IActionResult> Index(string searchString)
        {
            var query = _context.TrieuChungs.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(tc => tc.TenTC.Contains(searchString) || tc.MoTa.Contains(searchString));
            }
            
            ViewData["CurrentSearch"] = searchString;
            var danhSachTC = await query.ToListAsync();
            return View(danhSachTC);
        }

        // 1. GET: Hiển thị giao diện Form thêm mới
        public IActionResult Create()
        {
            return View();
        }

        // 2. POST: Xử lý dữ liệu từ Form gửi lên và lưu vào DB
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenTC,MoTa")] TrieuChung trieuChung)
        {
            // Bỏ qua kiểm tra các property liên kết để tránh lỗi ngầm
            ModelState.Remove("TrieuChungLois");

            // Kiểm tra dữ liệu hợp lệ (không bị bỏ trống tên triệu chứng)
            if (ModelState.IsValid)
            {
                _context.Add(trieuChung); // Thêm dữ liệu vào bộ nhớ tạm
                await _context.SaveChangesAsync(); // Lưu chính thức xuống SQL Server
                
                // Lưu thành công thì điều hướng quay về trang danh sách
                return RedirectToAction(nameof(Index)); 
            }
            
            // Nếu dữ liệu không hợp lệ (ví dụ: để trống tên), hiển thị lại Form kèm thông báo lỗi
            return View(trieuChung);
        }
        // ----------------- CHỨC NĂNG SỬA (EDIT) -----------------

        // 3. GET: Lấy dữ liệu cũ hiển thị lên Form Sửa
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trieuChung = await _context.TrieuChungs.FindAsync(id);
            if (trieuChung == null) return NotFound();

            return View(trieuChung);
        }

        // 4. POST: Nhận dữ liệu mới và cập nhật xuống Database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTC,TenTC,MoTa")] TrieuChung trieuChung)
        {
            if (id != trieuChung.MaTC) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trieuChung);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TrieuChungs.Any(e => e.MaTC == trieuChung.MaTC)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trieuChung);
        }

        // ----------------- CHỨC NĂNG XÓA (DELETE) -----------------

        // 5. POST: Thực thi xóa dữ liệu (Dùng POST để bảo mật, tránh bị xóa nhầm qua đường link GET)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trieuChung = await _context.TrieuChungs.FindAsync(id);
            if (trieuChung != null)
            {
                _context.TrieuChungs.Remove(trieuChung);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}