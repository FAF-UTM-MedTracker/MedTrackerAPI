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
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public string Note {  get; set; }
       
        [ForeignKey("User")]
        public int DoctorID { get; set; }
        public User Doctor { get; set; }

        public ICollection<Patient_Treatment> Patient_Treatment { get; set; }
        public ICollection<Treatment_Medication> Treatment_Medications { get; set; }

    }
}