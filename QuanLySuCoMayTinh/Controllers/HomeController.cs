using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLySuCoMayTinh.Models;
using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Data;

namespace QuanLySuCoMayTinh.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [AllowAnonymous]
    public async Task<IActionResult> TriggerAI([FromServices] System.Threading.Channels.Channel<QuanLySuCoMayTinh.Services.AITrainingRequest> channel, [FromServices] QuanLySuCoMayTinh.Data.ApplicationDbContext dbContext)
    {
        var job = new QuanLySuCoMayTinh.Models.AITrainingJob { TrangThai = "Pending", NgayBatDau = System.DateTime.Now };
        dbContext.AITrainingJobs.Add(job);
        await dbContext.SaveChangesAsync();
        await channel.Writer.WriteAsync(new QuanLySuCoMayTinh.Services.AITrainingRequest { JobId = job.MaTacVu, RequestedBy = "System" });
        return Content("Triggered AI Training Job " + job.MaTacVu);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetNotificationCounts()
    {
        int countPhieu = await _context.PhieuTiepNhans.CountAsync(p => p.TrangThai == "Chờ khám");
        int countDanhGia = await _context.DanhGiaDichVus.CountAsync();
        return Json(new { tickets = countPhieu, reviews = countDanhGia });
    }

    [HttpGet]
    public async Task<IActionResult> GetNotificationsHtml()
    {
        var recentPhieus = await _context.PhieuTiepNhans
            .Include(p => p.ThietBi).ThenInclude(t => t.KhachHang)
            .Where(p => p.TrangThai == "Chờ khám")
            .OrderByDescending(p => p.NgayNhan)
            .Take(5).ToListAsync();

        var recentDanhGias = await _context.DanhGiaDichVus
            .Include(d => d.PhieuTiepNhan).ThenInclude(p => p.ThietBi).ThenInclude(t => t.KhachHang)
            .OrderByDescending(d => d.NgayDG)
            .Take(3).ToListAsync();

        var model = Tuple.Create<IEnumerable<QuanLySuCoMayTinh.Models.PhieuTiepNhan>, IEnumerable<QuanLySuCoMayTinh.Models.DanhGiaDichVu>>(recentPhieus, recentDanhGias);
        return PartialView("_NotificationListPartial", model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GlobalSearch(string q)
    {
        if (string.IsNullOrWhiteSpace(q)) return Json(new object[0]);
        q = q.Trim().ToLower();

        // Tìm trong PhieuTiepNhan: ID phiếu, Tên Thiết Bị, Tên Khách Hàng, Số điện thoại
        var query = _context.PhieuTiepNhans
            .Include(p => p.ThietBi).ThenInclude(t => t.KhachHang)
            .Include(p => p.MaLoiNavigation)
            .AsQueryable();

        bool isNumeric = int.TryParse(q.Replace("hd-", ""), out int maPhieu);

        var results = await query
            .Where(p => 
                (isNumeric && p.MaPhieu == maPhieu) ||
                (p.ThietBi != null && p.ThietBi.TenTB.ToLower().Contains(q)) ||
                (p.ThietBi != null && p.ThietBi.KhachHang != null && (p.ThietBi.KhachHang.HoTen.ToLower().Contains(q) || p.ThietBi.KhachHang.SDT.Contains(q))) ||
                (p.MaLoiNavigation != null && p.MaLoiNavigation.TenLoi.ToLower().Contains(q))
            )
            .OrderByDescending(p => p.NgayNhan)
            .Take(5)
            .Select(p => new {
                id = p.MaPhieu,
                title = $"HD-{p.MaPhieu:D4} - {p.ThietBi!.KhachHang!.HoTen}",
                subtitle = p.ThietBi.TenTB,
                status = p.TrangThai,
                url = Url.Action("Details", "PhieuTiepNhan", new { id = p.MaPhieu })
            })
            .ToListAsync();

        return Json(results);
    }
    /// <summary>
    /// Trang Landing — công khai, không cần đăng nhập.
    /// Nếu đã đăng nhập thì chuyển thẳng vào Dashboard.
    /// </summary>
    [AllowAnonymous]
    public IActionResult Landing()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
            return RedirectToAction("Index");

        return View();
    }

    /// <summary>
    /// Trang Welcome — màn hình chào mừng đầu tiên khi mở app.
    /// </summary>
    [AllowAnonymous]
    public IActionResult Welcome()
    {
        return View();
    }

    /// <summary>
    /// Dashboard — yêu cầu đã đăng nhập.
    /// </summary>
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
