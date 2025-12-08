using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBXEMPHIMHKTMOVIE.Models
{
    public class Episode
    {
        public int EpisodeId { get; set; }
        public int SeriesId { get; set; }

        public int EpisodeNumber { get; set; }   // 🔹 Số tập
        public string Title { get; set; }
        public string VideoUrl { get; set; }     // Khớp với cột VideoUrl trong DB
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("SeriesId")]
        public virtual Series Series { get; set; }
    }
}
