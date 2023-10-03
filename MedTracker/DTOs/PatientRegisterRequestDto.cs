using System.ComponentModel.DataAnnotations;

namespace MedTracker.DTOs
{
    public class PatientRegisterRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string UPassword { get; set; }
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName {get; set;}
        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set;}

        [Required(ErrorMessage = "TellNumber is required")]
        public string PhoneNumber { get; set;}

        [Required(ErrorMessage = "Birth Date is required")]
        public DateTime DateofBirth { get; set;}

    }
}
