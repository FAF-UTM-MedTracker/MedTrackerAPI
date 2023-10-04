using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedTracker.Models
{
    public class NotesP
    {
        [Key]
        [Required]
        public int IdUser { get; set; }
        public Doctor Doctor { get; set; }
        public string Notes {  get; set; }
        public Patient Patient { get; set; }
    }
}
