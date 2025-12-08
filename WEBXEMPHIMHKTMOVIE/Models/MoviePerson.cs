using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBXEMPHIMHKTMOVIE.Models
{
    [Table("MoviePersons")]
    public class MoviePerson
    {
        [Key, Column(Order = 0)]
        public int MovieId { get; set; }

        [Key, Column(Order = 1)]
        public int PersonId { get; set; }

        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; }

        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; }
    }
}
