using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NemeShop.Models;
using System.Net;
using System.Threading.Tasks;

namespace NemeShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyTapHoaContext _context;
        public AccountController(QuanLyTapHoaContext context) => _context = context;

        [HttpGet]
        public IActionResult Login()
        {
            // Nếu đã đăng nhập thì chuyển về home
            if (HttpContext.Session.GetInt32("MaKh") != null ||
                !string.IsNullOrEmpty(HttpContext.Session.GetString("AdminId")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View("~/Views/User/Account/Login.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string matKhau)
        {
            // Khách hàng
            var kh = await _context.KhachHangs.FirstOrDefaultAsync(x => x.Email == email && x.MaKh > 0 && x.MatKhau == matKhau);
            if (kh != null)
            {
                // STORE MaKh as int
                HttpContext.Session.SetInt32("MaKh", kh.MaKh);
                HttpContext.Session.SetString("UserEmail", kh.Email ?? "");
                HttpContext.Session.SetString("HoTen", WebUtility.HtmlEncode(kh.HoTen) ?? "");
                HttpContext.Session.SetString("UserRole", "Customer");
                HttpContext.Session.SetString("ShowWelcome", "true");

                return RedirectToAction("Index", "Home");
            }

            // Admin
            var user = await _context.QuanTriViens.FirstOrDefaultAsync(x => x.Email == email && x.MatKhau == matKhau);
            if (user != null)
            {
                HttpContext.Session.SetString("AdminEmail", user.Email);
                HttpContext.Session.SetString("AdminId", user.MaQtv.ToString());
                HttpContext.Session.SetString("HoTen", WebUtility.HtmlEncode(user.HoTen) ?? "Quản trị viên");
                HttpContext.Session.SetString("UserRole", "Administrator");
                HttpContext.Session.SetString("ShowWelcome", "true");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai email hoặc mật khẩu. Vui lòng thử lại.";
            return View("~/Views/User/Account/Login.cshtml");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("MaKh") != null ||
                !string.IsNullOrEmpty(HttpContext.Session.GetString("AdminId")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View("~/Views/User/Account/Register.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string HoTen, string Email, string MatKhau, string DienThoai, string DiaChi)
        {
            var existingEmail = await _context.KhachHangs.FirstOrDefaultAsync(x => x.Email == Email);
            if (existingEmail != null)
            {
                ViewBag.Error = "Email đã được sử dụng. Vui lòng chọn email khác.";
                return View("~/Views/User/Account/Register.cshtml");
            }

            if (MatKhau.Length < 6)
            {
                ViewBag.Error = "Mật khẩu phải có ít nhất 6 ký tự.";
                return View("~/Views/User/Account/Register.cshtml");
            }

            var newId = _context.KhachHangs.Any() ? _context.KhachHangs.Max(x => x.MaKh) + 1 : 1;

            var khachHang = new KhachHang
            {
                MaKh = newId,
                HoTen = HoTen,
                Email = Email,
                MatKhau = MatKhau,
                DienThoai = DienThoai,
                DiaChi = DiaChi,
                TrangThai = true
            };

            _context.KhachHangs.Add(khachHang);
            await _context.SaveChangesAsync();

            // set session as int
            HttpContext.Session.SetInt32("MaKh", khachHang.MaKh);
            HttpContext.Session.SetString("UserEmail", khachHang.Email ?? "");
            HttpContext.Session.SetString("HoTen", WebUtility.HtmlEncode(khachHang.HoTen) ?? "");
            HttpContext.Session.SetString("UserRole", "Customer");
            HttpContext.Session.SetString("ShowWelcome", "true");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult HideWelcomeMessage()
        {
            HttpContext.Session.Remove("ShowWelcome");
            return Ok();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home", new { logout = "success" });
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View("~/Views/User/Account/AccessDenied.cshtml");
        }
    }
}
