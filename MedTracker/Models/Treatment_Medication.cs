using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedTracker.Models
{
    public class Treatment_Medication
    {
        [ForeignKey("Treatment")]
        
        public int IdTreatment { get; set; }
        public Treatment Treatment { get; set; }

        [ForeignKey("Medication")]
        
        public int IdMedication { get; set; }
        public Medication Medication { get; set; }
    }
}
