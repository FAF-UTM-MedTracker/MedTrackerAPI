using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedTracker.Models
{
    [Table("Medication")]
    public class Medication
    {
        [Key]
        public int IdMedication { get; set; }
        public string PName { get; set; }
        public string MDescription { get; set; }
        public DateTime Start_Time {  get; set; }
        public DateTime End_Time { get; set; }
        public DateTime TimeUse { get; set; }
        public double Quantity { get; set; }

        public ICollection<Treatment_Medication> Treatment_Medications { get; set; }
    }
}
