using MovieWebApp.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace MovieWebApp.Controllers
{
    public class AccountController : Controller
    {
        private MovieWebDbContext db = new MovieWebDbContext();

        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            // Tìm user trong database
            var user = db.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password);
            if (user != null)
            {
                // Set cookie để duy trì login
                FormsAuthentication.SetAuthCookie(user.Username, false);

                // Lưu thông tin vào Session để layout hiển thị
                Session["UserId"] = user.UserId;
                Session["Username"] = user.Username;
                Session["FullName"] = user.FullName;
                Session["Role"] = user.Role;

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
            return View();
        }

        // GET: Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }


        // GET: Register
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra username trùng
                if (db.Users.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                // Kiểm tra email trùng
                if (db.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("", "Email đã tồn tại.");
                    return View(model);
                }

                model.Role = "User"; // mặc định là User
                model.CreatedAt = System.DateTime.Now;

                db.Users.Add(model);
                db.SaveChanges();

                // Sau khi lưu thì login luôn
                FormsAuthentication.SetAuthCookie(model.Username, false);
                Session["UserId"] = model.UserId;
                Session["Username"] = model.Username;
                Session["FullName"] = model.FullName;
                Session["Role"] = model.Role;

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}
