using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBXEMPHIMHKTMOVIE.Models
{
    public class Movie
    {
        public int MovieId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int ReleaseYear { get; set; }

        public int Duration { get; set; }

        public string PosterPath { get; set; }

        public string VideoPath { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsSeries { get; set; }

        public int ViewCount { get; set; } = 0;

        public int? GenreId { get; set; }

        [ForeignKey("GenreId")]
        public virtual Genre Genre { get; set; }

        public virtual ICollection<MovieGenre> MovieGenres { get; set; }
        public virtual ICollection<MoviePerson> MoviePersons { get; set; }
        public virtual ICollection<WatchHistory> WatchHistories { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }

        public Movie()
        {
            MovieGenres = new HashSet<MovieGenre>();
            MoviePersons = new HashSet<MoviePerson>();
            WatchHistories = new HashSet<WatchHistory>();
            Favorites = new HashSet<Favorite>();
        }
    }
}
