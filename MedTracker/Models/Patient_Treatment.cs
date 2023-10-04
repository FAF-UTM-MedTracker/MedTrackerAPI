using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedTracker.Models
{
    public class Patient_Treatment
    {
        [ForeignKey("User")]
        [Required]
        public int IdUser { get; set; }
        public User Patient { get; set; }
        
        [ForeignKey("Treatment")]
        [Required]
        public int IdTreatment { get; set; }
        public Treatment Treatment { get; set;}
    }
}
