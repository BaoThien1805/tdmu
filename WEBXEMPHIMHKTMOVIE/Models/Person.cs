using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WEBXEMPHIMHKTMOVIE.Models
{
    [Table("Persons")]
    public class Person
    {
        [Key]
        public int PersonId { get; set; }

        [Required, StringLength(100)]
        public string FullName { get; set; }

        [StringLength(50)]
        public string Role { get; set; } // Actor, Director, Writer, etc.

        public virtual ICollection<MoviePerson> MoviePersons { get; set; }
    }
}
