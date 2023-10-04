namespace MedTracker.DTOs
{
    public class AddMedicationDbo
    {
        public int IdTreatment {  get; set; }
        public string PName { get; set; }
        public string MDescription { get; set; }
        public DateTime Start_Time { get; set; }
        public DateTime End_Time { get; set; }
        public DateTime TimeUse { get; set; }
        public double Quantity { get; set; }
    }
}
