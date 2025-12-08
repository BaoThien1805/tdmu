using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WEBXEMPHIMHKTMOVIE.Models;
using BCrypt.Net;

namespace WEBXEMPHIMHKTMOVIE.Controllers
{
    public class AccountController : Controller
    {
        private readonly MovieWebDbContext db = new MovieWebDbContext();

        // =============================
        // 🔹 GET: Login
        // =============================
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // =============================
        // 🔹 POST: Login
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string Username, string Password)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ Email/Tên đăng nhập và Mật khẩu.";
                return View();
            }

            // 🔍 Tìm người dùng theo Email hoặc Username
            var user = db.Users.FirstOrDefault(u => u.Email == Username || u.FullName == Username);
            if (user == null)
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng!";
                return View();
            }

            // 🔑 Kiểm tra mật khẩu (ưu tiên BCrypt)
            bool isPasswordValid = false;
            try
            {
                isPasswordValid = BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash);
            }
            catch
            {
                // nếu dữ liệu cũ chưa mã hóa
                isPasswordValid = (Password == user.PasswordHash);
            }

            if (!isPasswordValid)
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng!";
                return View();
            }

            // ✅ Đăng nhập thành công
            FormsAuthentication.SetAuthCookie(user.Email, false);
            Session["UserId"] = user.UserId;
            Session["Email"] = user.Email;
            Session["FullName"] = user.FullName;
            Session["Role"] = user.Role;
            // ============================
            // ✅ VIP LOGIC
            // ============================
            bool isVip = user.IsVip &&
                        (user.VipExpiredAt == null || user.VipExpiredAt > DateTime.Now);

            Session["IsVip"] = isVip;
            Session["VipExpiredAt"] = user.VipExpiredAt;

            // 🔁 Điều hướng theo vai trò
            if (!string.IsNullOrEmpty(user.Role) && user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            else
                return RedirectToAction("Index", "Home");
        }

        // =============================
        // 🔹 Logout
        // =============================
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // =============================
        // 🔹 GET: Register
        // =============================
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // =============================
        // 🔹 POST: Register
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string FullName, string Email, string PasswordHash, string PasswordConfirm)
        {
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(PasswordHash))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin.");
                return View();
            }

            if (PasswordHash != PasswordConfirm)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp.");
                return View();
            }

            if (db.Users.Any(u => u.Email == Email))
            {
                ModelState.AddModelError("", "Email đã tồn tại.");
                return View();
            }

            var user = new User
            {
                FullName = FullName,
                Email = Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(PasswordHash),
                Role = "User",
                IsVip = false,
                VipExpiredAt = null,
                CreatedAt = DateTime.Now
            };

            db.Users.Add(user);
            db.SaveChanges();

            // ✅ Tự động đăng nhập
            FormsAuthentication.SetAuthCookie(user.Email, false);
            Session["UserId"] = user.UserId;
            Session["Email"] = user.Email;
            Session["FullName"] = user.FullName;
            Session["Role"] = user.Role;

            return RedirectToAction("Index", "Home");
        }
        // =============================
        // 🔥 GET: Register VIP
        // =============================
        [Authorize]
        public ActionResult RegisterVip()
        {
            return View();
        }

        // =============================
        // 🔥 POST: Register VIP
        // =============================
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterVip(int months)
        {
            int userId = (int)Session["UserId"];
            var user = db.Users.Find(userId);

            if (user == null)
                return RedirectToAction("Login");

            user.IsVip = true;

            // Nếu đang VIP thì cộng thêm, nếu không thì tính từ hôm nay
            if (user.VipExpiredAt != null && user.VipExpiredAt > DateTime.Now)
                user.VipExpiredAt = user.VipExpiredAt.Value.AddMonths(months);
            else
                user.VipExpiredAt = DateTime.Now.AddMonths(months);

            db.SaveChanges();

            // ✅ Cập nhật lại session
            Session["IsVip"] = true;
            Session["VipExpiredAt"] = user.VipExpiredAt;

            TempData["Success"] = "Đăng ký VIP thành công!";
            return RedirectToAction("Index", "Home");
        }

    }
}
