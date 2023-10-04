using MedTracker.DTOs;
using MedTracker.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace MedTracker.Services
{
    public class AuthentificatorService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        private const string TokenSecret = "ThePillTrackerSecretKeyIsThisOneRightHere";
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);

        public AuthentificatorService(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public RegistrationResultDto RegisterUser(DoctorRegisterRequestDto requestDto)
        {
            try
            {
                // Check if the email is already taken
                if (IsEmailTaken(requestDto.Email))
                {
                    return new RegistrationResultDto
                    {
                        Success = false,
                        Message = "Email address is already in use."
                    };
                }

                // Create a new user entity and save it to the database
                var user = new User
                {
                    Email = requestDto.Email,
                    UPassword = requestDto.UPassword, // Note: You should hash and salt the password before storing it
                    IsDoctor = requestDto.IsDoctor // Assuming your DTO has a Doctor property
                };
                var doctor = new Doctor
                {
                    FirstName = requestDto.FirstName,
                    LastName = requestDto.LastName,
                    PhoneNumber = requestDto.PhoneNumber,
                    User = user
                };
                
                _dbContext.Users.Add(user);
                _dbContext.Doctors.Add(doctor);
                _dbContext.SaveChanges();

                return new RegistrationResultDto
                {
                    Success = true,
                    Message = "Registration successful"
                };
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"An error occurred during registration: {ex.Message}");

                return new RegistrationResultDto
                {
                    Success = false,
                    Message = "An error occurred during registration."
                };
            }
        }
        public RegistrationResultDto RegisterUser(PatientRegisterRequestDto requestDto)
        {
            try
            {
                // Check if the email is already taken
                if (IsEmailTaken(requestDto.Email))
                {
                    return new RegistrationResultDto
                    {
                        Success = false,
                        Message = "Email address is already in use."
                    };
                }

                // Create a new user entity and save it to the database
                //var hashedPassword = BCrypt.Net.BCrypt.HashPassword(requestDto.UPassword);
                var hashedPassword = requestDto.UPassword;
                var user = new User
                {
                    Email = requestDto.Email,
                    UPassword = hashedPassword,
                };
                var patient = new Patient
                {
                    FirstName = requestDto.FirstName,
                    LastName = requestDto.LastName,
                    PhoneNumber = requestDto.PhoneNumber,
                    Dateofbirth = requestDto.Dateofbirth,
                    User = user
                };

                _dbContext.Users.Add(user);
                _dbContext.Patients.Add(patient);
                _dbContext.SaveChanges();

                return new RegistrationResultDto
                {
                    Success = true,
                    Message = "Registration successful"
                };
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"An error occurred during registration: {ex.Message}");

                return new RegistrationResultDto
                {
                    Success = false,
                    Message = "An error occurred during registration."
                };
            }
        }

        public string? LoginUser(LoginRequestDto requestDto)
        {
            try
            {
                // Find the user by email
                var user = _dbContext.Users.First(u => u.Email == requestDto.Email);

                if (user == null)
                    return null;

                //var hashedPassword = BCrypt.Net.BCrypt.HashPassword(requestDto.UPassword);
                var hashedPassword = requestDto.UPassword;
                if (user.UPassword == hashedPassword)
                    return GenerateJwt(user.IdUser);
                else
                    return null;
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"An error occurred during login: {ex.Message}");
                return null;
            }
        }

        static string GenerateJwt(int userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(TokenSecret);

            var claims = new List<Claim>
            {
                new ("userId", userId.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TokenLifetime),
                Issuer = "https://pillTracker.com/",
                Audience = "https://medtrackerapi.azurewebsites.net",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private bool IsEmailTaken(string email)
        {
            return _dbContext.Users.Any(u => u.Email == email);
        }

    }
}
