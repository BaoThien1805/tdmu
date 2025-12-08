using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WEBXEMPHIMHKTMOVIE.Models;

namespace WEBXEMPHIMHKTMOVIE.Areas.Admin.Controllers
{
    public class GenresController : Controller
    {
        private readonly MovieWebDbContext db = new MovieWebDbContext();

        // ============================
        // 🔹 DANH SÁCH THỂ LOẠI
        // ============================
        public ActionResult Index()
        {
            var genres = db.Genres.ToList();
            return View(genres);
        }

        // ============================
        // 🔹 CHI TIẾT THỂ LOẠI
        // ============================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var genre = db.Genres.Find(id);
            if (genre == null)
                return HttpNotFound();

            return View(genre);
        }

        // ============================
        // 🔹 THÊM MỚI THỂ LOẠI
        // ============================
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Genre genre)
        {
            if (ModelState.IsValid)
            {
                db.Genres.Add(genre);
                db.SaveChanges();
                TempData["Success"] = "✅ Thêm thể loại mới thành công!";
                return RedirectToAction("Index");
            }
            return View(genre);
        }

        // ============================
        // 🔹 SỬA THỂ LOẠI
        // ============================
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var genre = db.Genres.Find(id);
            if (genre == null)
                return HttpNotFound();

            return View(genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Genre genre)
        {
            if (ModelState.IsValid)
            {
                var existing = db.Genres.Find(genre.GenreId);
                if (existing == null)
                    return HttpNotFound();

                existing.GenreName = genre.GenreName;
                db.SaveChanges();

                TempData["Success"] = "💾 Cập nhật thể loại thành công!";
                return RedirectToAction("Index");
            }
            return View(genre);
        }

        // ============================
        // 🔹 XÓA THỂ LOẠI
        // ============================
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var genre = db.Genres.Find(id);
            if (genre == null)
                return HttpNotFound();

            return View(genre);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var genre = db.Genres.Find(id);
            if (genre == null)
                return HttpNotFound();

            try
            {
                // Kiểm tra xem có phim nào đang dùng thể loại này không
                bool isUsed = db.Movies.Any(m => m.GenreId == id);
                if (isUsed)
                {
                    TempData["Error"] = "❌ Không thể xóa thể loại vì đang được sử dụng trong danh sách phim.";
                    return RedirectToAction("Index");
                }

                // Nếu không bị dùng -> xóa bình thường
                db.Genres.Remove(genre);
                db.SaveChanges();

                TempData["Success"] = "🗑️ Xóa thể loại thành công!";
            }
            catch
            {
                // Lỗi gì cũng chỉ hiển thị 1 thông báo chung
                TempData["Error"] = "❌ Không thể xóa thể loại vì đang được sử dụng trong danh sách phim.";
            }

            return RedirectToAction("Index");
        }


    }
}
