using System;
using System.Collections.Generic;
using WEBXEMPHIMHKTMOVIE.Models;

namespace MovieWebApp.Models
{
    public class Movie
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }
        public int Duration { get; set; }
        public int GenreId { get; set; }
        public string PosterPath { get; set; } // sẽ lưu "~/Content/Images/tenfile.jpg"
        public string VideoPath { get; set; }  // đường dẫn file mp4
        public DateTime CreatedAt { get; set; }

        // ✅ Thêm cột mới để phân biệt phim bộ hay phim lẻ
        public bool IsSeries { get; set; }

        // ✅ Thêm cột đếm lượt xem
        public int ViewCount { get; set; } = 0;

        // Khóa ngoại
        public virtual Genre Genre { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }

    }
}
