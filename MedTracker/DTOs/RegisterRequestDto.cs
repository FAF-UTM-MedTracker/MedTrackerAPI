using System.ComponentModel.DataAnnotations;

namespace MedTracker.DTOs
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string UPassword { get; set; }

        public bool Doctor { get; set; }
    }
}
