using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Đảm bảo namespace này được thêm để sử dụng session
using TraCuuThongTinMVC.ViewModels;
using TraCuuThongTinMVC.Data;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace TraCuuThongTinMVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly SchoolContext db;
        public AdminController(SchoolContext conetxt)
        {
            db = conetxt;
        }
        // Trang quản trị chính
        public IActionResult Index()
        {
            string username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Admin");
            }

            ViewBag.Username = username;
            return View();
        }

        // Trang đăng nhập (GET)
        public IActionResult Login()
        {
            return View();
        }

        // Xử lý đăng nhập (POST)
        [HttpPost]
        public async Task<IActionResult> Login(Admin model)
        {
            var user = await db.UserAdmins
                .FirstOrDefaultAsync(u => u.UserNm == model.UserNm && u.Passwork == model.Passwork);

            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserNm)
        };

                var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
                await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "InfSches");
            }

            ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return View();
        }

        // Xử lý đăng xuất
        public IActionResult Logout()
        {
            // Xóa toàn bộ dữ liệu session
            HttpContext.Session.Clear();

            // Quay về trang đăng nhập
            return RedirectToAction("Login");
        }
    }
}
