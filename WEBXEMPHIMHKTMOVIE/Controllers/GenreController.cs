using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBXEMPHIMHKTMOVIE.Models;

namespace WEBXEMPHIMHKTMOVIE.Controllers
{
    public class GenreController : Controller
    {
        private readonly MovieWebDbContext db = new MovieWebDbContext();
        // GET: Genre
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult GenreDropdown()
        {
            var genres = db.Genres.OrderBy(g => g.GenreName).ToList();
            return PartialView("_GenreDropdown", genres);
        }
    }
}
