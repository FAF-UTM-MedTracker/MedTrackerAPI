namespace MedTracker.DTOs
{
    public class UpdateTreatmentDto
    {
        public int TreatmentId { get; set; }
        public string? Name { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Note { get; set; }
        public int? DoctorId { get; set; }
    }
}
