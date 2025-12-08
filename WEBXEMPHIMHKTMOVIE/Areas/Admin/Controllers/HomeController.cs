using WEBXEMPHIMHKTMOVIE.Models;
using System.Linq;
using System.Web.Mvc;

namespace WEBXEMPHIMHKTMOVIE.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly MovieWebDbContext db = new MovieWebDbContext();

        public ActionResult Index()
        {
            // 🔒 Kiểm tra quyền đăng nhập
            var role = (Session["Role"] ?? "").ToString().ToLower();
            if (role != "admin")
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // 📊 Thống kê
            ViewBag.UserCount = db.Users.Count();
            ViewBag.MovieCount = db.Movies.Count();
            ViewBag.GenreCount = db.Genres.Count();

            ViewBag.FullName = Session["FullName"] ?? "Quản trị viên";
            ViewBag.Title = "Trang quản trị";

            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account", new { area = "" });
        }
    }
}
