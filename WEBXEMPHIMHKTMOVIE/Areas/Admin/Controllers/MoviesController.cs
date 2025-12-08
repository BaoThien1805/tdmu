using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WEBXEMPHIMHKTMOVIE.Models;
using WEBXEMPHIMHKTMOVIE.Services;

namespace WEBXEMPHIMHKTMOVIE.Areas.Admin.Controllers
{
    public class MoviesController : Controller
    {
        private MovieWebDbContext db = new MovieWebDbContext();
        private readonly LogService _logService = new LogService();

        // ========== KIỂM TRA QUYỀN ADMIN ==========
        private bool IsAdmin()
        {
            return Session["Role"] != null && Session["Role"].ToString().ToLower() == "admin";
        }
        private int CurrentAdminId
        {
            get
            {
                return Session["UserId"] != null ? (int)Session["UserId"] : 0;
            }
        }

        // ========== DANH SÁCH PHIM ==========
        public ActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });
            //_logService.WriteLog(CurrentAdminId, "Xem danh sách phim");
            var movies = db.Movies.Include(m => m.MovieGenres.Select(g => g.Genre)).ToList();
            return View(movies);
        }

        // ========== CHI TIẾT ==========
        public ActionResult Details(int? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var movie = db.Movies
                          .Include(m => m.MovieGenres.Select(g => g.Genre))
                          .FirstOrDefault(m => m.MovieId == id);
            //_logService.WriteLog(CurrentAdminId, $"Xem chi tiết phim: {movie.Title}");
            if (movie == null) return HttpNotFound();

            return View(movie);
        }


        // ========== TẠO MỚI ==========
        [HttpGet]
        public ActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            ViewBag.Genres = db.Genres.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Movie movie, int[] selectedGenres, HttpPostedFileBase PosterFile)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            if (ModelState.IsValid)
            {
                // 🖼️ Lưu poster (nếu có)
                if (PosterFile != null && PosterFile.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(PosterFile.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    PosterFile.SaveAs(path);
                    movie.PosterPath = "~/Content/Images/" + fileName;
                }

                // 🕒 Gán thông tin mặc định
                movie.CreatedAt = DateTime.Now;
                movie.ViewCount = 0;

                // 💾 Lưu phim trước
                db.Movies.Add(movie);
                db.SaveChanges();
                if (movie.IsSeries)
                {
                    var newSeries = new Series
                    {
                        MovieId = movie.MovieId,
                        SeriesName = movie.Title + " - Season 1"
                    };

                    db.Series.Add(newSeries);
                    db.SaveChanges();
                }

                // 🎬 Lưu thể loại (nhiều – nhiều)
                if (selectedGenres != null && selectedGenres.Any())
                {
                    foreach (var genreId in selectedGenres)
                    {
                        db.MovieGenres.Add(new MovieGenre
                        {
                            MovieId = movie.MovieId,
                            GenreId = genreId
                        });
                    }
                    db.SaveChanges();
                }
                _logService.WriteLog(CurrentAdminId, $"Thêm phim mới: {movie.Title}");
                TempData["SuccessMessage"] = "Thêm phim thành công!";
                return RedirectToAction("Index");
            }

            // ❌ Nếu lỗi, load lại danh sách thể loại
            ViewBag.Genres = db.Genres.ToList();
            return View(movie);
        }

        // ========== SỬA ==========
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var movie = db.Movies
                          .Include(m => m.MovieGenres)
                          .FirstOrDefault(m => m.MovieId == id);

            if (movie == null) return HttpNotFound();

            ViewBag.Genres = db.Genres.ToList();
            ViewBag.SelectedGenres = movie.MovieGenres.Select(g => g.GenreId).ToArray();

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Movie movie, int[] selectedGenres, HttpPostedFileBase PosterFile)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            if (!ModelState.IsValid)
            {
                ViewBag.Genres = db.Genres.ToList();
                ViewBag.SelectedGenres = selectedGenres;
                return View(movie);
            }

            var existingMovie = db.Movies
                .Include(m => m.MovieGenres)
                .FirstOrDefault(m => m.MovieId == movie.MovieId);

            if (existingMovie == null)
                return HttpNotFound();

            // ✅ Cập nhật thông tin
            existingMovie.Title = movie.Title;
            existingMovie.Description = movie.Description;
            existingMovie.ReleaseYear = movie.ReleaseYear;
            existingMovie.Duration = movie.Duration;
            existingMovie.VideoPath = movie.VideoPath;
            existingMovie.IsSeries = movie.IsSeries;

            // ✅ Xử lý ảnh (nếu có upload mới)
            if (PosterFile != null && PosterFile.ContentLength > 0)
            {
                string folderPath = Server.MapPath("~/Content/Images/");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(PosterFile.FileName);
                string savePath = Path.Combine(folderPath, fileName);
                PosterFile.SaveAs(savePath);

                existingMovie.PosterPath = "~/Content/Images/" + fileName;
            }

            // ✅ Cập nhật thể loại (n-n)
            db.MovieGenres.RemoveRange(existingMovie.MovieGenres);
            if (selectedGenres != null)
            {
                foreach (var gid in selectedGenres)
                {
                    db.MovieGenres.Add(new MovieGenre
                    {
                        MovieId = existingMovie.MovieId,
                        GenreId = gid
                    });
                }
            }

            db.SaveChanges();
            _logService.WriteLog(CurrentAdminId, $"Chỉnh sửa phim: {existingMovie.Title}");
            TempData["SuccessMessage"] = "Cập nhật phim thành công!";
            return RedirectToAction("Index");
        }



        // ========== XÓA ==========
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var movie = db.Movies
                          .Include(m => m.MovieGenres.Select(g => g.Genre))
                          .FirstOrDefault(m => m.MovieId == id);

            if (movie == null) return HttpNotFound();

            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var movie = db.Movies.Find(id);
            if (movie == null)
                return HttpNotFound();

            // 🔹 XÓA COMMENTS
            var comments = db.Comments.Where(c => c.MovieId == id).ToList();
            if (comments.Any())
                db.Comments.RemoveRange(comments);

            // 🔹 XÓA RATINGS
            var ratings = db.Ratings.Where(r => r.MovieId == id).ToList();
            if (ratings.Any())
                db.Ratings.RemoveRange(ratings);

            // 🔹 XÓA WATCH HISTORIES
            var histories = db.WatchHistories.Where(h => h.MovieId == id).ToList();
            if (histories.Any())
                db.WatchHistories.RemoveRange(histories);

            // 🔹 XÓA FAVORITES
            var favorites = db.Favorites.Where(f => f.MovieId == id).ToList();
            if (favorites.Any())
                db.Favorites.RemoveRange(favorites);

            // 🔹 XÓA SERIES
            var series = db.Series.Where(s => s.MovieId == id).ToList();
            if (series.Any())
                db.Series.RemoveRange(series);
            // Episodes sẽ tự xoá nếu FK dùng cascade

            // 🔹 CUỐI CÙNG XÓA MOVIE
            db.Movies.Remove(movie);

            db.SaveChanges();

            _logService.WriteLog(CurrentAdminId, $"Xóa phim: {movie.Title}");

            return RedirectToAction("Index");
        }



    }
}
