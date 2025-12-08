using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBXEMPHIMHKTMOVIE.Models
{
    [Table("Genres")]
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }

        [Required]
        [StringLength(100)]
        public string GenreName { get; set; }

        // ===========================
        // 🔹 Quan hệ
        // ===========================
        // Một thể loại có thể có nhiều MovieGenre (n-n)
        public virtual ICollection<MovieGenre> MovieGenres { get; set; }

        public Genre()
        {
            MovieGenres = new HashSet<MovieGenre>();
        }
    }
}
