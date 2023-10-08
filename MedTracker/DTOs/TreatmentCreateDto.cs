namespace MedTracker.DTOs
{
    public class TreatmentCreateDto
    {
        public string TName { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Note { get; set; }
        public List<int> MedicationIds { get; set; }


        
    }
}
