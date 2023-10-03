using System.ComponentModel.DataAnnotations;

namespace MedTracker.DTOs
{
    
    public class DoctorRegisterRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string UPassword { get; set; }

        public bool IsDoctor { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "A Telephone number  is required")]
        public string PhoneNumber { get; set; }

    }
}
