using Microsoft.AspNetCore.Mvc;
using MedTracker.DTOs;
using MedTracker.Services;

namespace MedTracker.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticatorController : ControllerBase
    {
        private readonly AuthentificatorService _authentificatorService;

        public AuthenticatorController(AuthentificatorService authentificatorService)
        {
            _authentificatorService = authentificatorService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequestDto requestDto)
        {
            var registrationResult = _authentificatorService.RegisterUser(requestDto);

            if (registrationResult.Success)
            {
                return Ok(new { Message = registrationResult.Message });
            }
            else
            {
                return BadRequest(new { Message = registrationResult.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto requestDto)
        {
            var loginResult = _authentificatorService.LoginUser(requestDto);

            if (loginResult.Success)
            {
                return Ok(new { Message = loginResult.Message });
            }
            else
            {
                return BadRequest(new { Message = loginResult.Message });
            }
        }
    }
}
