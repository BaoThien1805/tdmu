using MovieWebApp.Models;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity;

namespace MovieWebApp.Controllers
{
    public class UsersController : Controller
    {
        private MovieWebDbContext db = new MovieWebDbContext();

        // ========== DANH SÁCH ==========
        public ActionResult Index()
        {
            var users = db.Users.ToList();
            return View(users);
        }

        // ========== CHI TIẾT ==========
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(id);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        // ========== TẠO ==========
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                user.Role = "User"; // mặc định khi tạo
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // ========== SỬA ==========
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(id);
            if (user == null) return HttpNotFound();

            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = db.Users.Find(user.UserId);
                if (existingUser == null) return HttpNotFound();

                // ⚡ Gán thủ công các field được chỉnh sửa
                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.Username = user.Username;
                existingUser.Role = user.Role;
                // ❌ Không override PasswordHash, CreatedAt, ... nếu không có trong form

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }


        // ========== XÓA ==========
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(id);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var role = (HttpContext.Session["Role"] ?? "").ToString();
            if (string.IsNullOrEmpty(role) || role != "Admin")
            {
                filterContext.Result = new RedirectResult("/Account/Login");
            }
            base.OnActionExecuting(filterContext);
        }


    }
}
