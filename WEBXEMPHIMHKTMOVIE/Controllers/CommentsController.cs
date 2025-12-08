using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBXEMPHIMHKTMOVIE.Models;

namespace WEBXEMPHIMHKTMOVIE.Controllers
{
    public class CommentsController : Controller
    {
        // GET: Comments
        private MovieWebDbContext db = new MovieWebDbContext();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int movieId, string content)
        {
            if (Session["UserId"] == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để bình luận." });
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return Json(new { success = false, message = "Nội dung bình luận không được để trống." });
            }

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
                content = content,
                createdAt = comment.CreatedAt.ToString("dd/MM/yyyy HH:mm")
            });
        }

        public PartialViewResult List(int movieId)
        {
            var comments = db.Comments
                .Where(c => c.MovieId == movieId)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return PartialView("_CommentList", comments);
        }
    }
}