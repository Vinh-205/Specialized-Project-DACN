using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Data;
using QuanLySuCoMayTinh.Models;

namespace QuanLySuCoMayTinh.Controllers
{
    [Authorize]
    public partial class ChiTietSuaChuaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChiTietSuaChuaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemDichVu(int MaPhieu, string TenHangMuc, decimal ChiPhiDV)
        {
            if (string.IsNullOrWhiteSpace(TenHangMuc))
            {
                TempData["ErrorMsg"] = "Vui lòng nhập tên dịch vụ sửa chữa.";
                return RedirectToAction("Details", "PhieuTiepNhan", new { id = MaPhieu });
            }

            if (ChiPhiDV < 0) ChiPhiDV = 0;

            var ct = new ChiTietSuaChua
            {
                MaPhieu = MaPhieu,
                TenHangMuc = TenHangMuc.Trim(),
                ChiPhi = ChiPhiDV
            };

            _context.ChiTietSuaChuas.Add(ct);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã thêm dịch vụ sửa chữa '{ct.TenHangMuc}' thành công!";
            return RedirectToAction("Details", "PhieuTiepNhan", new { id = MaPhieu });
        }
    }
}
