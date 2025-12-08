using System;
using System.Linq;
using System.Web.Mvc;
using WEBXEMPHIMHKTMOVIE.Models;
using System.Data.Entity;

namespace WEBXEMPHIMHKTMOVIE.Areas.Admin.Controllers
{
    public class CommentsController : Controller
    {
        private MovieWebDbContext db = new MovieWebDbContext();

        // ================== DANH SÁCH COMMENTS ==================
        [HttpGet]
        public ActionResult Index()
        {
            // Lấy danh sách comments kèm thông tin User và Movie
            var comments = db.Comments
                             .Include("User")
                             .Include("Movie")
                             .OrderByDescending(c => c.CreatedAt)
                             .ToList();

            return View(comments);
        }

        // ================== XÓA COMMENT ==================
        [HttpPost]
        public JsonResult Delete(int commentId)
        {
            try
            {
                var comment = db.Comments.Find(commentId);
                if (comment == null)
                    return Json(new { success = false, message = "Comment không tồn tại!" });

                db.Comments.Remove(comment);
                db.SaveChanges();

                return Json(new { success = true, message = "Xóa comment thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // ================== CHỈNH SỬA NỘI DUNG COMMENT ==================
        [HttpPost]
        public JsonResult EditContent(int commentId, string content)
        {
            try
            {
                var comment = db.Comments.Find(commentId);
                if (comment == null)
                    return Json(new { success = false, message = "Comment không tồn tại!" });

                comment.Content = content;
                db.SaveChanges();

                return Json(new { success = true, message = "Cập nhật comment thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
