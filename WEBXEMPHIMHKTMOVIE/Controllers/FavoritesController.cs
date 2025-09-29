using System;
using System.Linq;
using System.Web.Mvc;
using MovieWebApp.Models;
using WEBXEMPHIMHKTMOVIE.Models;

namespace MovieWebApp.Controllers
{
    public class FavoritesController : Controller
    {
        private MovieWebDbContext db = new MovieWebDbContext();

        [HttpPost]
        public ActionResult ToggleFavorite(int movieId)
        {
            int userId = Convert.ToInt32(Session["UserId"] ?? 0);
            if (userId == 0)
                return Json(new { success = false, message = "Vui lòng đăng nhập" });

            var existing = db.Favorites
                             .FirstOrDefault(f => f.UserId == userId && f.MovieId == movieId);

            if (existing != null)
            {
                // Nếu đã tồn tại thì xóa (bỏ yêu thích)
                db.Favorites.Remove(existing);
                db.SaveChanges();
                return Json(new { success = true, isFavorite = false });
            }
            else
            {
                // Nếu chưa có thì thêm mới
                var fav = new Favorite { UserId = userId, MovieId = movieId, CreatedAt = DateTime.Now };
                db.Favorites.Add(fav);
                db.SaveChanges();
                return Json(new { success = true, isFavorite = true });
            }
        }

        public ActionResult MyFavorites()
        {
            int userId = Convert.ToInt32(Session["UserId"] ?? 0);
            if (userId == 0) return RedirectToAction("Login", "Account");

            var movies = db.Favorites
                           .Where(f => f.UserId == userId)
                           .Select(f => f.Movie)
                           .ToList();

            return View(movies);
        }
    }
}
