using System;
using System.Collections.Generic;
using WEBXEMPHIMHKTMOVIE.Models;

namespace MovieWebApp.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }

    }
}
