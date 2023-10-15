namespace MedTracker.DTOs
{
    public class UpdateTreatmentStatusDto
    {
        public int IdTreatment { get; set; }
        public string Status { get; set; } 
        public string Note { get; set; }
    }
}
