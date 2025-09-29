using MovieWebApp.Models;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MovieWebApp.Controllers
{
    public class GenresController : Controller
    {
        private MovieWebDbContext db = new MovieWebDbContext();

        // ========== DANH SÁCH ==========
        public ActionResult Index()
        {
            return View(db.Genres.ToList());
        }

        // ========== CHI TIẾT ==========
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var genre = db.Genres.Find(id);
            if (genre == null) return HttpNotFound();

            return View(genre);
        }

        // ========== TẠO ==========
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
                return RedirectToAction("Index");
            }
            return View(genre);
        }

        // ========== SỬA ==========
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var genre = db.Genres.Find(id);
            if (genre == null) return HttpNotFound();

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
                {
                    return HttpNotFound();
                }
                existing.GenreName = genre.GenreName;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(genre);
        }


        // ========== XÓA ==========
        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var genre = db.Genres.Find(id);
            if (genre == null) return HttpNotFound();

            return View(genre);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var genre = db.Genres.Find(id);
            if (genre == null)
            {
                return HttpNotFound();
            }
            db.Genres.Remove(genre);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}
