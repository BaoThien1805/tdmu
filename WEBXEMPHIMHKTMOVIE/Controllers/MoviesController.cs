using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WEBXEMPHIMHKTMOVIE.Models;

namespace WEBXEMPHIMHKTMOVIE.Controllers
{
    public class MoviesController : Controller
    {
        private MovieWebDbContext db = new MovieWebDbContext();

        // ========== DANH SÁCH PHIM ==========
        public ActionResult Index(int page = 1, int pageSize = 12)
        {
            var movies = db.Movies
                           .Include(m => m.MovieGenres.Select(g => g.Genre))
                           .OrderByDescending(m => m.CreatedAt)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling((double)db.Movies.Count() / pageSize);
            return View(movies);
        }

        // ========== CHI TIẾT PHIM ==========
        public ActionResult Details(int id)
        {
            var movie = db.Movies
                .Include(m => m.MovieGenres.Select(g => g.Genre))
                .Include(m => m.MoviePersons.Select(p => p.Person))
                .FirstOrDefault(m => m.MovieId == id);

            if (movie == null)
                return HttpNotFound();

            movie.ViewCount++;
            db.SaveChanges();

            var genreIds = movie.MovieGenres.Select(g => g.GenreId).ToList();
            ViewBag.RelatedMovies = db.Movies
                .Include(m => m.MovieGenres.Select(g => g.Genre))
                .Where(m => m.MovieId != id && m.MovieGenres.Any(g => genreIds.Contains(g.GenreId)))
                .OrderByDescending(m => m.ViewCount)
                .Take(4)
                .ToList();

            int? userId = Session["UserId"] as int?;
            if (userId != null)
            {
                ViewBag.UserFavorites = db.Favorites
                    .Where(f => f.UserId == userId)
                    .Select(f => f.MovieId)
                    .ToList();
                var history = db.WatchHistories
            .FirstOrDefault(h => h.MovieId == id && h.UserId == userId);

                if (history == null)
                {
                    // tạo mới
                    db.WatchHistories.Add(new WatchHistory
                    {
                        UserId = userId.Value,
                        MovieId = id,
                        WatchedAt = DateTime.Now
                    });
                }
                else
                {
                    // cập nhật thời gian xem
                    history.WatchedAt = DateTime.Now;
                }

                db.SaveChanges();

            }

            var ratings = db.Ratings.Where(r => r.MovieId == id);
            double avgScore = ratings.Any() ? ratings.Average(r => r.Score) : 0;
            int totalRatings = ratings.Count();

            double userRating = 0;
            if (userId != null)
            {
                var userRate = db.Ratings.FirstOrDefault(r => r.MovieId == id && r.UserId == userId);
                if (userRate != null)
                    userRating = userRate.Score;
            }

            ViewBag.AverageRating = Math.Round(avgScore * 2, 1);
            ViewBag.TotalRatings = totalRatings;
            ViewBag.AverageRatingPercent = (int)Math.Round((avgScore / 5.0) * 100);
            ViewBag.UserRating = userRating;

            return View(movie);
        }

        // ========== GỬI ĐÁNH GIÁ ==========
        [HttpPost]
        public ActionResult RateMovie(int movieId, double score)
        {
            if (Session["UserId"] == null)
                return Json(new { success = false, message = "Bạn cần đăng nhập để đánh giá." });

            int userId = (int)Session["UserId"];
            int finalScore = (int)Math.Round(score);
            finalScore = Math.Min(5, Math.Max(1, finalScore));

            var existing = db.Ratings.FirstOrDefault(r => r.MovieId == movieId && r.UserId == userId);
            if (existing != null)
                existing.Score = finalScore;
            else
                db.Ratings.Add(new Rating
                {
                    MovieId = movieId,
                    UserId = userId,
                    Score = finalScore,
                    CreatedAt = DateTime.Now
                });

            db.SaveChanges();
            return Json(new { success = true, message = "Đánh giá của bạn đã được lưu!" });
        }

        public ActionResult Watch(int id, int? episodeId)
        {
            // Lấy movie kèm thể loại
            var movie = db.Movies
                .Include(m => m.MovieGenres.Select(g => g.Genre))
                .FirstOrDefault(m => m.MovieId == id);

            if (movie == null)
                return HttpNotFound();

            // Tăng view
            movie.ViewCount++;
            db.SaveChanges();

            // Nếu là phim bộ
            if (movie.IsSeries)
            {
                // Lấy series theo MovieId
                var series = db.Series
                    .Include(s => s.Episodes)
                    .FirstOrDefault(s => s.MovieId == id);

                if (series != null)
                {
                    // Lấy toàn bộ tập
                    var episodes = series.Episodes
                        .OrderBy(e => e.EpisodeNumber)
                        .ToList();

                    ViewBag.Episodes = episodes;

                    // Nếu phim không có tập → vẫn load view
                    if (!episodes.Any())
                    {
                        ViewBag.CurrentEpisodeId = null;
                        return View(movie);
                    }

                    // Nếu chọn tập cụ thể
                    if (episodeId.HasValue)
                    {
                        var ep = episodes.FirstOrDefault(e => e.EpisodeId == episodeId.Value);
                        if (ep != null)
                        {
                            movie.VideoPath = ep.VideoUrl;
                            ViewBag.CurrentEpisodeId = ep.EpisodeId;
                        }
                    }
                    else
                    {
                        // Nếu không chọn → phát tập đầu tiên
                        var firstEp = episodes.First();
                        movie.VideoPath = firstEp.VideoUrl;
                        ViewBag.CurrentEpisodeId = firstEp.EpisodeId;
                    }
                }
                else
                {
                    // Nếu không tìm thấy series nhưng IsSeries = true
                    ViewBag.Episodes = new List<Episode>();
                    ViewBag.CurrentEpisodeId = null;
                }
            }

            return View(movie);
        }




        // ========== PHIM THEO THỂ LOẠI ==========
        public ActionResult ByGenre(int id, int page = 1, int pageSize = 12)
        {
            var genre = db.Genres.Find(id);
            if (genre == null) return HttpNotFound();

            var movies = db.Movies
                .Include(m => m.MovieGenres.Select(g => g.Genre))
                .Where(m => m.MovieGenres.Any(g => g.GenreId == id))
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalMovies = db.Movies.Count(m => m.MovieGenres.Any(g => g.GenreId == id));

            ViewBag.GenreId = id;
            ViewBag.GenreName = genre.GenreName;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling((double)totalMovies / pageSize);

            return View(movies);
        }

        // ========== PHIM LẺ ==========
        public ActionResult Single(int page = 1, int pageSize = 12)
        {
            var movies = db.Movies
                .Include(m => m.MovieGenres.Select(g => g.Genre))
                .Where(m => !m.IsSeries)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalMovies = db.Movies.Count(m => !m.IsSeries);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling((double)totalMovies / pageSize);
            return View(movies);
        }

        // ========== PHIM BỘ ==========
        public ActionResult Series(int page = 1, int pageSize = 12)
        {
            var movies = db.Movies
                .Include(m => m.MovieGenres.Select(g => g.Genre))
                .Where(m => m.IsSeries)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalMovies = db.Movies.Count(m => m.IsSeries);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = Math.Ceiling((double)totalMovies / pageSize);
            return View(movies);
        }

        // ========== TÌM KIẾM ==========
        public ActionResult Search(string query, int page = 1)
        {
            if (string.IsNullOrEmpty(query))
            {
                ViewBag.Query = "";
                ViewBag.TotalResults = 0;
                ViewBag.CurrentPage = 1;
                ViewBag.TotalPages = 1;
                return View(new List<Movie>());
            }

            int pageSize = 12;

            var movies = db.Movies
                .Where(m => m.Title.Contains(query))
                .OrderByDescending(m => m.CreatedAt);

            int total = movies.Count();
            int totalPages = (int)Math.Ceiling((double)total / pageSize);

            // Giới hạn page đứng trong khoảng
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var result = movies
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Gán ViewBag (dùng kiểu nullable để tránh lỗi RuntimeBinder)
            ViewBag.Query = query ?? "";
            ViewBag.TotalResults = total;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages == 0 ? 1 : totalPages;

            return View(result);
        }


        // ========== DANH SÁCH ==========
        public ActionResult List()
        {
            var movies = db.Movies
                .Include(m => m.MovieGenres.Select(g => g.Genre))
                .OrderByDescending(m => m.CreatedAt)
                .ToList();

            return View(movies);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
        [HttpPost]
        public ActionResult AddComment(int movieId, string content)
        {
            if (Session["UserId"] == null)
                return Json(new { success = false, message = "Bạn cần đăng nhập để bình luận." });

            if (string.IsNullOrWhiteSpace(content))
                return Json(new { success = false, message = "Nội dung bình luận không được để trống." });

            int userId = (int)Session["UserId"];
            var comment = new Comment
            {
                MovieId = movieId,
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.Now
            };

            db.Comments.Add(comment);
            db.SaveChanges();

            var user = db.Users.Find(userId);

            return Json(new
            {
                success = true,
                userName = user.FullName,
                content = comment.Content
            });
        }

        [HttpGet]
        public ActionResult GetComments(int movieId)
        {
            var comments = db.Comments
                .Include(c => c.User)
                .Where(c => c.MovieId == movieId)
                .OrderByDescending(c => c.CreatedAt)
                .ToList() // 👉 Lấy dữ liệu vào bộ nhớ trước
                .Select(c => new
                {
                    userName = c.User.FullName,
                    content = c.Content,
                    createdAt = c.CreatedAt.ToString("dd/MM/yyyy HH:mm") // Bây giờ OK
                })
                .ToList();

            return Json(comments, JsonRequestBehavior.AllowGet);
        }


    }
}
