namespace MedTracker.DTOs
{
    public class TreatmentCreateDto
    {
        public string TName { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public string Note { get; set; }
        public int DoctorID { get; set; }
    }
}
