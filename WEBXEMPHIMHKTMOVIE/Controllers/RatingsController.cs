using System;
using System.Linq;
using System.Web.Mvc;
using WEBXEMPHIMHKTMOVIE.Models;

namespace WEBXEMPHIMHKTMOVIE.Controllers
{
    public class RatingsController : Controller
    {
        private readonly MovieWebDbContext db = new MovieWebDbContext();

        // ========== GỬI ĐÁNH GIÁ ==========
        [HttpPost]
public ActionResult RateMovie(int movieId, double score)
{
    if (Session["UserId"] == null)
    {
        return Json(new { success = false, message = "Bạn cần đăng nhập để đánh giá." });
    }

    int userId = (int)Session["UserId"];

    // Ép giá trị score vào khung 1–5 (vì bảng có CHECK)
    int finalScore = (int)Math.Round(score);
    if (finalScore < 1) finalScore = 1;
    if (finalScore > 5) finalScore = 5;

    // Kiểm tra xem user này đã đánh giá phim này chưa
    var existing = db.Ratings.FirstOrDefault(r => r.MovieId == movieId && r.UserId == userId);

    if (existing != null)
    {
        // Cập nhật điểm (không cần cột thời gian vì đã có CreatedAt mặc định)
        existing.Score = finalScore;
    }
    else
    {
        // Thêm mới
        var newRating = new Rating
        {
            MovieId = movieId,
            UserId = userId,
            Score = finalScore,
            CreatedAt = DateTime.Now // tương thích với cột trong DB
        };
        db.Ratings.Add(newRating);
    }

    db.SaveChanges();

    return Json(new { success = true, message = "Đánh giá của bạn đã được lưu!" });
}



    }
}
