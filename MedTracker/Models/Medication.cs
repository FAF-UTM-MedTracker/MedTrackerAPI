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
        public string Description { get; set; }
        public DateTime Start {  get; set; }
        public DateTime End { get; set; }
        public DateTime TimeUse { get; set; }
        public float Quantity { get; set; }

        public List<Treatment_Medication> Treatment_Medications { get; set; }
    }
}
