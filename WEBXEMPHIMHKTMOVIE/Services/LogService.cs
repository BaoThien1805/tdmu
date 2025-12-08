using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WEBXEMPHIMHKTMOVIE.Models;

namespace WEBXEMPHIMHKTMOVIE.Services
{
    public class LogService
    {
        private readonly MovieWebDbContext db;

        public LogService()
        {
            db = new MovieWebDbContext();
        }

        public void WriteLog(int adminId, string action)
        {
            var log = new AdminLog
            {
                AdminId = adminId,
                Action = action,
                LogTime = DateTime.Now
            };

            db.AdminLogs.Add(log);
            db.SaveChanges();
        }
    }
}