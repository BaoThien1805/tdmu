using System;
using System.Linq;
using System.Web.Mvc;
using WEBXEMPHIMHKTMOVIE.Models;
using System.Data.Entity;

namespace WEBXEMPHIMHKTMOVIE.Areas.Admin.Controllers
{
    public class AdminRatingsController : Controller
    {
        private MovieWebDbContext db = new MovieWebDbContext();

        public ActionResult Index()
        {
            var ratings = db.Ratings
                .Include("User")
                .Include("Movie")
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return View(ratings);
        }

        // UPDATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Update(int ratingId, int score)
        {
            try
            {
                var rating = db.Ratings.Find(ratingId);
                if (rating == null)
                    return Json(new { success = false, message = "Không tìm thấy đánh giá!" });

                rating.Score = score;
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int ratingId)
        {
            try
            {
                var rating = db.Ratings.Find(ratingId);
                if (rating == null)
                    return Json(new { success = false, message = "Không tìm thấy đánh giá!" });

                db.Ratings.Remove(rating);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
