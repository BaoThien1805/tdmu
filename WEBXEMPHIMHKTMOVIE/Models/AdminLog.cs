using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBXEMPHIMHKTMOVIE.Models
{
    [Table("AdminLogs")]
    public class AdminLog
    {
        [Key]
        [Column("AdminLogId")]   // 🔥 map đúng khóa chính
        public int AdminLogId { get; set; }

        public int? AdminId { get; set; }  // 🔥 Cho phép null vì ON DELETE SET NULL

        [StringLength(200)]
        public string Action { get; set; }

        [Column("LogTime")]   // 🔥 map đúng tên cột DB
        public DateTime LogTime { get; set; } = DateTime.Now;

        [ForeignKey("AdminId")]
        public virtual User Admin { get; set; }
    }
}
