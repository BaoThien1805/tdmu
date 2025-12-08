using System.ComponentModel.DataAnnotations.Schema;

namespace WEBXEMPHIMHKTMOVIE.Models
{
    [Table("MovieGenres")]
    public class MovieGenre
    {
        public int MovieId { get; set; }
        public int GenreId { get; set; }

        // 🔹 Navigation properties
        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; }

        [ForeignKey("GenreId")]
        public virtual Genre Genre { get; set; }
    }
}
