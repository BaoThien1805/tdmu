using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using WEBXEMPHIMHKTMOVIE.Models;

namespace WEBXEMPHIMHKTMOVIE.Models
{
    public class MovieWebDbContext : DbContext
    {
        public MovieWebDbContext() : base("MovieWebDB_New") { }

        // ========================
        // DbSet cho tất cả bảng
        // ========================
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<WatchHistory> WatchHistories { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<MoviePerson> MoviePersons { get; set; }
        public DbSet<AdminLog> AdminLogs { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<AdsBanner> AdsBanners { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================
            // ⚙️ Cấu hình cơ bản
            // ========================
            // Tắt mặc định cascade delete để tránh lỗi multiple paths
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            // ========================
            // 🎬 MovieGenre (n-n)
            // ========================
            modelBuilder.Entity<MovieGenre>()
                .HasKey(mg => new { mg.MovieId, mg.GenreId });

            modelBuilder.Entity<MovieGenre>()
                .HasRequired(mg => mg.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(mg => mg.MovieId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MovieGenre>()
                .HasRequired(mg => mg.Genre)
                .WithMany(g => g.MovieGenres)
                .HasForeignKey(mg => mg.GenreId)
                .WillCascadeOnDelete(false);

            // ========================
            // 👥 MoviePerson (n-n)
            // ========================
            modelBuilder.Entity<MoviePerson>()
                .HasKey(mp => new { mp.MovieId, mp.PersonId });

            modelBuilder.Entity<MoviePerson>()
                .HasRequired(mp => mp.Movie)
                .WithMany(m => m.MoviePersons)
                .HasForeignKey(mp => mp.MovieId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<MoviePerson>()
                .HasRequired(mp => mp.Person)
                .WithMany(p => p.MoviePersons)
                .HasForeignKey(mp => mp.PersonId)
                .WillCascadeOnDelete(false);

            // ========================
            // 👁 WatchHistory
            // ========================
            modelBuilder.Entity<WatchHistory>()
                .HasKey(w => w.WatchHistoryId);

            modelBuilder.Entity<WatchHistory>()
                .HasRequired(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<WatchHistory>()
                .HasRequired(w => w.Movie)
                .WithMany(m => m.WatchHistories)
                .HasForeignKey(w => w.MovieId)
                .WillCascadeOnDelete(false);

            // ========================
            // 🧾 AdminLog
            // ========================
            modelBuilder.Entity<AdminLog>()
                .HasKey(a => a.AdminLogId);   // map đúng khóa chính
            modelBuilder.Entity<AdminLog>()
                .HasOptional(a => a.Admin)    // AdminId cho phép NULL
                .WithMany()
                .HasForeignKey(a => a.AdminId)
                .WillCascadeOnDelete(false);  // không xóa log khi xóa user
        }
    }
}
