using System;
using System.Linq;
using System.Web.Mvc;
using WEBXEMPHIMHKTMOVIE.Models;

namespace WEBXEMPHIMHKTMOVIE.Controllers
{
    public class WatchHistoryController : Controller
    {
        private MovieWebDbContext db = new MovieWebDbContext();

        // =============================
        // 1️⃣ HIỂN THỊ LỊCH SỬ XEM
        // =============================
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Auth");

            int userId = (int)Session["UserId"];

            var history = db.WatchHistories
                .Where(h => h.UserId == userId)
                .OrderByDescending(h => h.WatchedAt)
                .ToList();

            return View(history);
        }


        // =============================
        // 2️⃣ API GHI NHẬN LỊCH SỬ XEM
        // (Gọi khi vào trang Details)
        // =============================
        [HttpPost]
        public JsonResult Add(int movieId)
        {
            if (Session["UserId"] == null)
                return Json(new { success = false, message = "Chưa đăng nhập" });

            int userId = (int)Session["UserId"];

            // xem có tồn tại chưa
            var existing = db.WatchHistories
                .FirstOrDefault(h => h.UserId == userId && h.MovieId == movieId);

            if (existing != null)
            {
                // update thời gian xem
                existing.WatchedAt = DateTime.Now;
            }
            else
            {
                // tạo mới
                var h = new WatchHistory
                {
                    UserId = userId,
                    MovieId = movieId,
                    WatchedAt = DateTime.Now
                };
                db.WatchHistories.Add(h);
            }

            db.SaveChanges();

            return Json(new { success = true });
        }


        // =============================
        // 3️⃣ XÓA 1 LỊCH SỬ
        // =============================
        [HttpPost]
        public JsonResult Delete(int id)
        {
            if (Session["UserId"] == null)
                return Json(new { success = false });

            var item = db.WatchHistories.Find(id);
            if (item == null)
                return Json(new { success = false });

            db.WatchHistories.Remove(item);
            db.SaveChanges();

            return Json(new { success = true });
        }


        // =============================
        // 4️⃣ XÓA TOÀN BỘ LỊCH SỬ
        // =============================
        [HttpPost]
        public JsonResult ClearAll()
        {
            if (Session["UserId"] == null)
                return Json(new { success = false });

            int userId = (int)Session["UserId"];

            var all = db.WatchHistories.Where(h => h.UserId == userId);
            db.WatchHistories.RemoveRange(all);
            db.SaveChanges();

            return Json(new { success = true });
        }
    }
}
