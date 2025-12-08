using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WEBXEMPHIMHKTMOVIE.Models
{
    [Table("AdsBanners")]
    public class AdsBanner
    {
        [Key]
        public int BannerId { get; set; }

        public string Title { get; set; }

        public string ImagePath { get; set; }

        public string TargetUrl { get; set; }

        public string Position { get; set; }

        public bool IsActive { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}