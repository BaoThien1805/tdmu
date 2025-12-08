using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBXEMPHIMHKTMOVIE.Models;

namespace WEBXEMPHIMHKTMOVIE.Areas.Admin.Controllers
{
    public class AdsBannersController : Controller
    {
        private MovieWebDbContext db = new MovieWebDbContext();

        // ===== INDEX =====
        public ActionResult Index()
        {
            return View();
        }

        // ===== AJAX: LẤY DANH SÁCH =====
        public JsonResult GetList()
        {
            var data = db.AdsBanners
                         .OrderByDescending(x => x.CreatedAt)
                         .ToList();

            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        // ===== AJAX: LẤY 1 BANNER (Edit) =====
        public JsonResult GetBanner(int id)
        {
            var banner = db.AdsBanners.Find(id);
            if (banner == null)
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);

            return Json(new { success = true, data = banner }, JsonRequestBehavior.AllowGet);
        }

        // ===== CREATE =====
        [HttpPost]
        public JsonResult CreateBanner()
        {
            try
            {
                var title = Request.Form["Title"];
                var targetUrl = Request.Form["TargetUrl"];
                var position = Request.Form["Position"];
                var isActive = Request.Form["IsActive"] == "true" || Request.Form["IsActive"] == "on";

                string imagePath = null;

                if (Request.Files.Count > 0)
                {
                    var file = Request.Files["img"];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Uploads/Banners/"), fileName);

                        if (!Directory.Exists(Server.MapPath("~/Uploads/Banners/")))
                            Directory.CreateDirectory(Server.MapPath("~/Uploads/Banners/"));

                        file.SaveAs(path);
                        imagePath = "/Uploads/Banners/" + fileName;
                    }
                }

                var banner = new AdsBanner
                {
                    Title = title,
                    TargetUrl = targetUrl,
                    Position = position,
                    IsActive = isActive,
                    ImagePath = imagePath,
                    CreatedAt = DateTime.Now
                };

                db.AdsBanners.Add(banner);
                db.SaveChanges();

                return Json(new { success = true, message = "Thêm banner thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // ===== UPDATE =====
        [HttpPost]
        public JsonResult UpdateBanner()
        {
            try
            {
                int bannerId = int.Parse(Request.Form["BannerId"]);
                var banner = db.AdsBanners.Find(bannerId);
                if (banner == null)
                    return Json(new { success = false, message = "Banner không tồn tại" });

                banner.Title = Request.Form["Title"];
                banner.TargetUrl = Request.Form["TargetUrl"];
                banner.Position = Request.Form["Position"];
                banner.IsActive = Request.Form["IsActive"] == "true" || Request.Form["IsActive"] == "on";

                if (Request.Files.Count > 0)
                {
                    var file = Request.Files["img"];
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Uploads/Banners/"), fileName);

                        if (!Directory.Exists(Server.MapPath("~/Uploads/Banners/")))
                            Directory.CreateDirectory(Server.MapPath("~/Uploads/Banners/"));

                        file.SaveAs(path);
                        banner.ImagePath = "/Uploads/Banners/" + fileName;
                    }
                }

                db.SaveChanges();
                return Json(new { success = true, message = "Cập nhật banner thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // ===== DELETE =====
        [HttpPost]
        public JsonResult DeleteBanner(int id)
        {
            try
            {
                var banner = db.AdsBanners.Find(id);
                if (banner == null)
                    return Json(new { success = false, message = "Banner không tồn tại" });

                // Xóa ảnh cũ nếu có
                if (!string.IsNullOrEmpty(banner.ImagePath))
                {
                    var fullPath = Server.MapPath(banner.ImagePath);
                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                }

                db.AdsBanners.Remove(banner);
                db.SaveChanges();

                return Json(new { success = true, message = "Xóa banner thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }
    }
}
