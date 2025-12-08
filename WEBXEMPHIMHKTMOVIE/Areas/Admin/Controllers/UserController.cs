using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
    using WEBXEMPHIMHKTMOVIE.Models;
using WEBXEMPHIMHKTMOVIE.Services;

namespace WEBXEMPHIMHKTMOVIE.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        private readonly MovieWebDbContext db = new MovieWebDbContext();
        private readonly LogService _log;

        // ============================
        // 🔹 DANH SÁCH NGƯỜI DÙNG
        // ============================
        public ActionResult Index()
        {
            var users = db.Users.ToList();
            // Trả view bằng đường dẫn tuyệt đối file trong project (TEST)
            return View(users);

        }

        // ============================
        // 🔹 XEM CHI TIẾT NGƯỜI DÙNG
        // ============================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound("Không tìm thấy người dùng.");

            return View(user);
        }

        // ============================
        // 🔹 THÊM NGƯỜI DÙNG
        // ============================
        [HttpGet]
        public ActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (!ModelState.IsValid)
                return View(user);

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                ModelState.AddModelError("PasswordHash", "Vui lòng nhập mật khẩu.");
                return View(user);
            }

            // ✅ Gán giá trị mặc định
            user.Role = string.IsNullOrEmpty(user.Role) ? "User" : user.Role;
            user.CreatedAt = DateTime.Now;

            db.Users.Add(user);
            db.SaveChanges();

            TempData["Success"] = "✅ Thêm người dùng thành công!";
            return RedirectToAction("Index");
        }

        // ============================
        // 🔹 CHỈNH SỬA NGƯỜI DÙNG
        // ============================
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Exclude = "PasswordHash")] User user)
        {
            ModelState.Remove("PasswordHash"); // 🩹 Bỏ validate field này

            if (ModelState.IsValid)
            {
                var existing = db.Users.Find(user.UserId);
                if (existing != null)
                {
                    existing.FullName = user.FullName;
                    existing.Email = user.Email;
                    existing.Role = user.Role;
                    db.SaveChanges();

                    TempData["Success"] = "Cập nhật thông tin người dùng thành công!";
                    return RedirectToAction("Index");
                }
            }

            TempData["Error"] = "Không thể lưu thay đổi. Vui lòng kiểm tra lại.";
            return View(user);
        }



        // ============================
        // 🔹 XÓA NGƯỜI DÙNG
        // ============================
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound("Không tìm thấy người dùng.");

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            db.Users.Remove(user);
            db.SaveChanges();

            TempData["Success"] = "🗑️ Xóa người dùng thành công!";
            return RedirectToAction("Index");
        }

        // ============================
        // 🔒 CHỈ ADMIN MỚI VÀO ĐƯỢC
        // ============================
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var role = (filterContext.HttpContext.Session["Role"] ?? "").ToString();
            if (!role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                filterContext.Result = new RedirectResult("/Account/Login");
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        // ============================
        // 🔹 GIẢI PHÓNG NGUỒN
        // ============================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
