using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBXEMPHIMHKTMOVIE.Models
{
    public class Series
    {
        [Key]
        public int SeriesId { get; set; }

        public int MovieId { get; set; }

        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; }

        public string SeriesName { get; set; }

        public virtual ICollection<Episode> Episodes { get; set; } = new HashSet<Episode>();
    }
}
