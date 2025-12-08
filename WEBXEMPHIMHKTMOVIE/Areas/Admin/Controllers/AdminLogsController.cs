using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEBXEMPHIMHKTMOVIE.Models;
using System.Data.Entity;


namespace WEBXEMPHIMHKTMOVIE.Areas.Admin.Controllers
{
    public class AdminLogsController : Controller
    {
        private MovieWebDbContext db = new MovieWebDbContext();
        // GET: Admin/AdminLogs
        public ActionResult Index()
        {
            return View();
        }

        // =======================
        // 2. AJAX LẤY LOG
        // =======================
        public ActionResult GetLogs()
        {
            var logs = db.AdminLogs
                .Include(l => l.Admin)
                .OrderByDescending(l => l.LogTime)
                .ToList()                // ⭐ Chuyển sang RAM trước rồi mới format
                .Select(l => new
                {
                    l.AdminLogId,
                    AdminName = l.Admin != null ? l.Admin.FullName : "(Đã xoá)",
                    l.Action,
                    LogTime = l.LogTime.ToString("yyyy-MM-dd HH:mm:ss")  // ⭐ Format ở đây
                })
                .ToList();

            return Json(logs, JsonRequestBehavior.AllowGet);
        }


        // =======================
        // 3. GHI LOG (gọi từ nơi khác)
        // =======================
        [NonAction]
        public void WriteLog(int? adminId, string action)
        {
            var log = new AdminLog
            {
                AdminId = adminId,
                Action = action,
                LogTime = System.DateTime.Now
            };

            db.AdminLogs.Add(log);
            db.SaveChanges();
        }
    }
}