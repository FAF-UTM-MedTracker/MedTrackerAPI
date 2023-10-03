using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace MedTracker.Models
{
    [Table("Treatment")]
    public class Treatment
    {
        [Key]
        public int IdTreatment { get; set; }
        public string TName { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Note {  get; set; }
       
        [ForeignKey("User")]
        
        public int IdUserD { get; set; }
        public User User { get; set; }

        public List<Patient_Treatment> Patient_Treatments { get; set; }
        public List<Treatment_Medication> Treatment_Medications { get; set; }

    }
}