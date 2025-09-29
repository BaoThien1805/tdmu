using MovieWebApp.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MovieWebApp.Controllers
{
    public class HomeController : Controller
    {
        private MovieWebDbContext db = new MovieWebDbContext();

        public ActionResult Index(int page = 1, int pageSize = 8)
        {
            // 🎬 Slider
            var sliderMovies = db.Movies
                .Where(m => m.PosterPath != null && m.PosterPath != "")
                .OrderByDescending(m => m.CreatedAt)
                .Take(5)
                .ToList();

            // 📂 Lấy tất cả thể loại
            var genres = db.Genres.ToList();

            // ⭐ Phim đề cử
            var featuredMovies = db.Movies
                .Where(m => m.PosterPath != null && m.PosterPath != "")
                .OrderByDescending(m => m.ViewCount)
                .Take(4)
                .ToList();

            // 📺 Tất cả phim để phân trang
            var movies = db.Movies
                .Where(m => m.PosterPath != null && m.PosterPath != "")
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 📄 Tính tổng số trang
            int totalMovies = db.Movies.Count(m => m.PosterPath != null && m.PosterPath != "");
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalMovies / pageSize);
            ViewBag.CurrentPage = page;

            // 📝 Nhóm phim theo thể loại (để hiển thị từng khối trong view)
            var allMovies = db.Movies
                .Where(m => m.PosterPath != null && m.PosterPath != "")
                .OrderByDescending(m => m.CreatedAt)
                .ToList();

            var moviesByGenre = genres.ToDictionary(
                g => g, // key: Genre
                g => allMovies.Where(m => m.GenreId == g.GenreId).Take(8).ToList() // value: List<Movie>
            );

            // Gửi dữ liệu sang View
            ViewBag.SliderMovies = sliderMovies;
            ViewBag.Genres = genres;
            ViewBag.FeaturedMovies = featuredMovies;
            ViewBag.MoviesByGenre = moviesByGenre;

            return View(movies); // Model chính vẫn có thể là danh sách chung
        }


    }
}
