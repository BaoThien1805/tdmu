using WEBXEMPHIMHKTMOVIE.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace WEBXEMPHIMHKTMOVIE.Controllers
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

            // ⭐ Phim đề cử (lấy theo ViewCount)
            var featuredMovies = db.Movies
                .Where(m => m.PosterPath != null && m.PosterPath != "")
                .OrderByDescending(m => m.ViewCount)
                .Take(4)
                .ToList();

            // 📺 Phim phân trang
            var movies = db.Movies
                .Where(m => m.PosterPath != null && m.PosterPath != "")
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 📄 Tổng số trang
            int totalMovies = db.Movies.Count(m => m.PosterPath != null && m.PosterPath != "");
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalMovies / pageSize);
            ViewBag.CurrentPage = page;

            // 📝 Lấy phim theo từng thể loại (dựa vào bảng trung gian MovieGenres)
            var moviesByGenre = genres.ToDictionary(
                g => g,
                g => db.MovieGenres
                        .Where(mg => mg.GenreId == g.GenreId)
                        .Select(mg => mg.Movie)
                        .Where(m => m.PosterPath != null && m.PosterPath != "")
                        .OrderByDescending(m => m.CreatedAt)
                        .Take(8)
                        .ToList()
            );

            // Gửi dữ liệu sang View
            ViewBag.SliderMovies = sliderMovies;
            ViewBag.Genres = genres;
            ViewBag.FeaturedMovies = featuredMovies;
            ViewBag.MoviesByGenre = moviesByGenre;
            // ----- Banner Ads -----
            ViewBag.BannersTop = db.AdsBanners
    .Where(b => b.Position == "HomeTop" && b.IsActive)
    .ToList();

            ViewBag.BannersMid = db.AdsBanners
                .Where(b => b.Position == "HomeSide" && b.IsActive)
                .ToList();

            ViewBag.BannersBottom = db.AdsBanners
                .Where(b => b.Position == "HomeBottom" && b.IsActive)
                .ToList();
            ViewBag.PopupBanner = db.AdsBanners
    .Where(b => b.Position == "HomePopup" && b.IsActive)
    .FirstOrDefault();

            return View(movies);
        }
    }
}
