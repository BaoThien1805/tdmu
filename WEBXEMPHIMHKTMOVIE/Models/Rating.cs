using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WEBXEMPHIMHKTMOVIE.Models
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int UserId { get; set; }
        public int MovieId { get; set; }
        public int Score { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User User { get; set; }
        public virtual Movie Movie { get; set; }
    }

}