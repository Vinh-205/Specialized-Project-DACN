using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySuCoMayTinh.Data;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using QuanLySuCoMayTinh.Models;

namespace QuanLySuCoMayTinh.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Nếu người dùng đã đăng nhập thì không cho vào lại trang Login mà đá về trang chủ
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var jsonResponse = Request.Headers["Accept"].ToString().Contains("application/json");

            // 1. Kiểm tra tài khoản có khớp với Database (Bảng USER)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password_Hash == password);

            if (user == null)
            {
                if (jsonResponse) return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không chính xác." });
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                return View();
            }

            if (!user.Status)
            {
                if (jsonResponse) return Unauthorized(new { message = "Tài khoản của bạn đã bị khóa." });
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa.");
                return View();
            }

            // 2. Lấy thông tin user để lưu vào Cookie (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.USERID.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, user.Role ?? "KTV")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            // 3. Tiến hành đăng nhập phiên làm việc
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            // 4. Chuyển hướng
            if (jsonResponse)
            {
                var destination = !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                    ? returnUrl : Url.Action("Index", "Home");
                return Json(new { success = true, redirectUrl = destination });
            }
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
            return RedirectToAction("Index", "Home"); // Chuyển về Dashboard
        }

        public async Task<IActionResult> Logout()
        {
            // Xoá Cookie xác thực để thực sự thoát phiên làm việc
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Chuyển hướng về trang Landing (giới thiệu hệ thống)
            return RedirectToAction("Landing", "Home");
        }

        // GET: /Auth/Profile
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Lấy ID của người dùng từ Cookie (NameIdentifier) để tìm chính xác trong DB
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return NotFound("Lỗi phiên đăng nhập!");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.USERID == userId);
            
            if (user == null) 
            {
                return NotFound("Không tìm thấy thông tin tài khoản!");
            }
            
            return View(user);
        }
    }
}
