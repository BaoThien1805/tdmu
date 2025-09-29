using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MovieWebApp.Models;

namespace WEBXEMPHIMHKTMOVIE.Models
{
    public class Favorite
    {
        [Key]
        public int FavoriteId { get; set; }

        // Khóa ngoại đến User
        public int UserId { get; set; }
        public virtual User User { get; set; }

        // Khóa ngoại đến Movie
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}