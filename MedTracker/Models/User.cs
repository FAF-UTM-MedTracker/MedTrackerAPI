using System.ComponentModel.DataAnnotations;

namespace MedTracker.Models
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }
        public string Email { get; set; }
        public string? Salt { get; set; }
        public string UPassword { get; set; }
        public bool IsDoctor { get; set; }  

        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }

        public ICollection<Patient_Treatment> Patient_Treatment { get; set; }
        public ICollection<Treatment> Treatments { get; set; }
    }
}
