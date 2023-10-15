namespace MedTracker.DTOs
{
    public class UpdateMedicationDto
    {
        public int IdMedication { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? TimeUse { get; set; }
        public double? Quantity { get; set; }

    }
}
