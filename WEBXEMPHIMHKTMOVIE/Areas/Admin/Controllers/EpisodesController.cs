using System;
using System.Linq;
using System.Web.Mvc;
using WEBXEMPHIMHKTMOVIE.Models;

namespace WEBXEMPHIMHKTMOVIE.Areas.Admin.Controllers
{
    public class EpisodesController : Controller
    {
        MovieWebDbContext db = new MovieWebDbContext();

        // =============================
        // 📌 TRANG QUẢN LÝ TẬP PHIM
        // =============================
        public ActionResult Index(int movieId)
        {
            if (movieId <= 0)
                return HttpNotFound("MovieId không hợp lệ");

            var movie = db.Movies.Find(movieId);
            if (movie == null)
                return HttpNotFound("Không tìm thấy phim");

            // 🔥 Nếu phim là phim lẻ → không cho vào Episodes
            if (!movie.IsSeries)
                return Content("Phim này không phải phim bộ!");

            // 🔍 Lấy series nếu có
            var series = db.Series.FirstOrDefault(s => s.MovieId == movieId);

            // ➕ Nếu chưa có → tạo mới
            if (series == null)
            {
                series = new Series
                {
                    MovieId = movieId,
                    SeriesName = movie.Title + " - Season 1"
                };
                db.Series.Add(series);
                db.SaveChanges();
            }

            // Gửi dữ liệu sang View
            ViewBag.Movie = movie;
            ViewBag.SeriesId = series.SeriesId;

            return View();
        }

        // =============================
        // 📌 LẤY DANH SÁCH TẬP – AJAX
        // =============================
        public ActionResult GetEpisodes(int seriesId)
        {
            var eps = db.Episodes
                        .Where(e => e.SeriesId == seriesId)
                        .OrderBy(e => e.EpisodeNumber)
                        .ToList();

            return PartialView("_EpisodeList", eps);
        }

        // =============================
        // 📌 TẠO TẬP MỚI – AJAX
        // =============================
        [HttpPost]
        public ActionResult CreateEpisodeAjax(int seriesId, string title, string videoUrl)
        {
            if (seriesId <= 0 || db.Series.Find(seriesId) == null)
                return Json(new { success = false, message = "Series không tồn tại!" });

            if (string.IsNullOrEmpty(videoUrl))
                return Json(new { success = false, message = "Video URL không được bỏ trống!" });

            // 🔢 Tự tính số tập tiếp theo
            int nextEp = db.Episodes
                           .Where(e => e.SeriesId == seriesId)
                           .Select(e => (int?)e.EpisodeNumber)
                           .Max() ?? 0;

            nextEp++;

            var ep = new Episode
            {
                SeriesId = seriesId,
                EpisodeNumber = nextEp,
                Title = string.IsNullOrEmpty(title) ? $"Tập {nextEp}" : title,
                VideoUrl = videoUrl,
                CreatedAt = DateTime.Now
            };

            db.Episodes.Add(ep);
            db.SaveChanges();

            return Json(new { success = true, message = "Tạo tập thành công!" });
        }

        // =============================
        // 📌 XÓA TẬP – AJAX
        // =============================
        [HttpPost]
        public ActionResult DeleteEpisodeAjax(int id)
        {
            var ep = db.Episodes.Find(id);
            if (ep == null)
                return Json(new { success = false, message = "Không tìm thấy tập!" });

            db.Episodes.Remove(ep);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa tập thành công!" });
        }
    }
}
