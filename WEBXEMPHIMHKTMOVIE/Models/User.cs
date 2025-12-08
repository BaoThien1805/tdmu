using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBXEMPHIMHKTMOVIE.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [Required, StringLength(100), EmailAddress]
        public string Email { get; set; }

        [Required, StringLength(255)]
        public string PasswordHash { get; set; }

        [StringLength(50)]
        public string Role { get; set; } = "User";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Relationship Example
        public virtual ICollection<Favorite> Favorites { get; set; }
        // =============================
        // 🔥 VIP FIELDS (NEW)
        // =============================

        /// <summary>
        /// User có phải là VIP hay không
        /// </summary>
        public bool IsVip { get; set; } = false;

        /// <summary>
        /// Ngày hết hạn VIP (null = VIP vĩnh viễn)
        /// </summary>
        public DateTime? VipExpiredAt { get; set; }

    }
}
