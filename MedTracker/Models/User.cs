using System.ComponentModel.DataAnnotations;

namespace MedTracker.Models
{
    public class User
    {
        [Key]
        public int IdUser { get; set; }
        public string Email { get; set; }
        public string UPassword { get; set; }
        public bool IsDoctor { get; set; }  

        public List<Doctor> Doctors { get; set; }
        public List<Patient> Patients { get; set; }

        public List<Patient_Treatment> Patient_Treatments { get; set; }
        public List<Treatment> Treatments { get; set; }
    }
}
