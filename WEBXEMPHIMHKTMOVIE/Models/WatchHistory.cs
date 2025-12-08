using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBXEMPHIMHKTMOVIE.Models
{
    [Table("WatchHistories")]
    public class WatchHistory
    {
        [Key]
        public int WatchHistoryId { get; set; }

        public int UserId { get; set; }
        public int MovieId { get; set; }
        public DateTime WatchedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("MovieId")]
        public virtual Movie Movie { get; set; }
    }
}
