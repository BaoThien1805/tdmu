using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MovieWebApp.Models;

namespace MovieWebApp.Controllers
{
    public class MoviesController : Controller
    {
        private MovieWebDbContext db = new MovieWebDbContext();

        // ========== DANH SÁCH PHIM ==========
        public ActionResult Index()
        {
            var movies = db.Movies.Include(m => m.Genre).ToList();

            // Nếu admin => view quản trị, nếu không => view mặc định (cards)
            if (Session["Role"] != null && Session["Role"].ToString().ToLower() == "admin")
            {
                return View("IndexAdmin", movies); // view quản trị
            }

            return View(movies); // view mặc định card poster
        }


        // ========== CHI TIẾT PHIM ==========
        public ActionResult Details(int id)
        {
            var movie = db.Movies.Include("Genre").FirstOrDefault(m => m.MovieId == id);
            if (movie == null) return HttpNotFound();

            // Lấy phim cùng thể loại (trừ chính nó)
            var relatedMovies = db.Movies
                                  .Where(m => m.GenreId == movie.GenreId && m.MovieId != id)
                                  .OrderByDescending(m => m.CreatedAt)
                                  .Take(4)
                                  .ToList();
            ViewBag.RelatedMovies = relatedMovies;

            // 🟢 Lấy danh sách phim yêu thích của user hiện tại
            int userId = Convert.ToInt32(Session["UserId"] ?? 0);
            if (userId != 0)
            {
                ViewBag.UserFavorites = db.Favorites
                                          .Where(f => f.UserId == userId)
                                          .Select(f => f.MovieId)
                                          .ToList();
            }
            else
            {
                ViewBag.UserFavorites = new List<int>();
            }

            return View(movie);
        }



        // ========== PHIM BỘ ==========
        [HttpGet]
        public ActionResult Series(int page = 1, int pageSize = 12)
        {
            var movies = db.Movies.Where(m => m.IsSeries)
                                  .OrderByDescending(m => m.CreatedAt)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToList();

            return View(movies); // ⚡ Trả về view mặc định: Views/Movies/Series.cshtml
        }

        // ========== PHIM LẺ ==========
        [HttpGet]
        public ActionResult Single(int page = 1, int pageSize = 12)
        {
            var movies = db.Movies.Where(m => !m.IsSeries)
                                  .OrderByDescending(m => m.CreatedAt)
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToList();

            return View(movies); // ⚡ Trả về Views/Movies/Single.cshtml
        }

        // ========== TÌM KIẾM ==========
        public ActionResult Search(string query, int page = 1, int pageSize = 12)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                TempData["Error"] = "Vui lòng nhập từ khóa trước khi tìm kiếm.";
                return RedirectToAction("Index", "Home");
            }

            var moviesQuery = db.Movies
                                .Where(m => m.Title.Contains(query));

            int totalResults = moviesQuery.Count();

            var movies = moviesQuery
                         .OrderByDescending(m => m.CreatedAt)
                         .Skip((page - 1) * pageSize)
                         .Take(pageSize)
                         .ToList();

            // Gửi dữ liệu sang View
            ViewBag.Query = query;
            ViewBag.TotalResults = totalResults;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalResults / pageSize);

            return View("Search", movies);
        }
        // ========== PHIM THEO THỂ LOẠI ==========
        [HttpGet]
        public ActionResult ByGenre(int id, int page = 1, int pageSize = 12)
        {
            var genre = db.Genres.Find(id);
            if (genre == null) return HttpNotFound();

            var movies = db.Movies
                           .Where(m => m.GenreId == id)
                           .OrderByDescending(m => m.CreatedAt)
                           .Skip((page - 1) * pageSize)
                           .Take(pageSize)
                           .ToList();

            // Phân trang
            int totalMovies = db.Movies.Count(m => m.GenreId == id);
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalMovies / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.GenreName = genre.GenreName;

            return View("ByGenre", movies);  // ⚡ tạo view ByGenre.cshtml
        }


        // ========== TẠO MỚI (Admin) ==========
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "GenreName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Movie movie, HttpPostedFileBase PosterFile)
        {
            if (ModelState.IsValid)
            {
                // Upload Poster (ảnh nhẹ)
                if (PosterFile != null && PosterFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(PosterFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    PosterFile.SaveAs(path);
                    movie.PosterPath = "~/Content/Images/" + fileName;
                }

                // ⚡ Video: Chỉ nhập link (Textbox trong form)
                // movie.VideoPath đã được bind từ form

                movie.CreatedAt = DateTime.Now;
                movie.ViewCount = 0;

                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "GenreName", movie.GenreId);
            return View(movie);
        }





        // ========== SỬA (Admin) ==========
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var movie = db.Movies.Find(id);
            if (movie == null) return HttpNotFound();

            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "GenreName", movie.GenreId);
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Movie movie, HttpPostedFileBase PosterFile)
        {
            if (ModelState.IsValid)
            {
                // Nếu có poster mới thì thay
                if (PosterFile != null && PosterFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(PosterFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    PosterFile.SaveAs(path);
                    movie.PosterPath = "~/Content/Images/" + fileName;
                }

                // ⚡ VideoPath nhập bằng textbox nên vẫn giữ nguyên

                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GenreId = new SelectList(db.Genres, "GenreId", "GenreName", movie.GenreId);
            return View(movie);
        }

        // ========== XÓA (Admin) ==========
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var movie = db.Movies.Include(m => m.Genre).FirstOrDefault(m => m.MovieId == id);
            if (movie == null) return HttpNotFound();
            return View(movie); // sẽ hiển thị Delete.cshtml
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var movie = db.Movies.Find(id);
            if (movie == null) return HttpNotFound();
            db.Movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // ========== Xem phim ==========
        public ActionResult Watch(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var movie = db.Movies.Include(m => m.Genre)
                                 .FirstOrDefault(m => m.MovieId == id);
            if (movie == null) return HttpNotFound();

            // tăng lượt xem
            movie.ViewCount++;
            db.SaveChanges();

            return View(movie); // Views/Movies/Watch.cshtml
        }


    }
}
