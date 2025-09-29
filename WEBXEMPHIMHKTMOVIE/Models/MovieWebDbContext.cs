using System.Data.Entity;
using WEBXEMPHIMHKTMOVIE.Models;

namespace MovieWebApp.Models
{
    public class MovieWebDbContext : DbContext
    {
        public MovieWebDbContext() : base("MovieWebDB") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
    }
}
