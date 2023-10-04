using Microsoft.AspNetCore.Mvc;
using MedTracker.DTOs;
using MedTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MedTracker.Controllers
{
    [ApiController]
    [Route("auth")]
    //[Authorize]
    public class AuthenticatorController : ControllerBase
    {
        private readonly AuthentificatorService _authentificatorService;

        public AuthenticatorController(AuthentificatorService authentificatorService)
        {
            _authentificatorService = authentificatorService;
        }

        [HttpPost("register/doctor")]
        public IActionResult Register([FromBody] DoctorRegisterRequestDto requestDto)
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
        [HttpPost("register/patient")]
        public IActionResult Register([FromBody] PatientRegisterRequestDto requestDto)
        {
            var registationResult = _authentificatorService.RegisterUser(requestDto);
            if (registationResult.Success)
            {
                return Ok(new {Message = registationResult.Message});
            }
            else
            {
                return BadRequest(new {Message = registationResult.Message});
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto requestDto)
        {
            var loginResult = _authentificatorService.LoginUser(requestDto);
            if (loginResult == null)
            {
                return BadRequest(new { Message = "Authentication error." });
            }
            else
            {
                return Ok(new { Message = "Authententication succedeed.", Jwt = loginResult });
            }
        }
    }
}
