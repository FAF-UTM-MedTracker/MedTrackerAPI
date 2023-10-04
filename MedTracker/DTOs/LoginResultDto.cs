using MedTracker.Models;

namespace MedTracker.DTOs
{
    public class LoginResultDto
    {
        public string Message { get; set; }
        public string Jwt { get; set; }
    }
}
