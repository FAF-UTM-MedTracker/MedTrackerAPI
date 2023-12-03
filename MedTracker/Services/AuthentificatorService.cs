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
                var generatedSalt = BCrypt.Net.BCrypt.GenerateSalt();
                var hashedSaltAndPass = BCrypt.Net.BCrypt.HashPassword(generatedSalt + requestDto.UPassword, generatedSalt);
                var user = new User
                {
                    Email = requestDto.Email,
                    Salt = generatedSalt,
                    UPassword = hashedSaltAndPass,
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
                var generatedSalt = BCrypt.Net.BCrypt.GenerateSalt();
                var hashedSaltAndPass = BCrypt.Net.BCrypt.HashPassword(generatedSalt + requestDto.UPassword, generatedSalt);
                var user = new User
                {
                    Email = requestDto.Email,
                    Salt = generatedSalt,
                    UPassword = hashedSaltAndPass,
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

                if (user.Salt.IsNullOrEmpty())
                {
                    if (user.UPassword == requestDto.UPassword)
                        return GenerateJwt(user.IdUser);
                }
                else
                {
                    var hashedSaltAndPassword = BCrypt.Net.BCrypt.HashPassword(user.Salt + requestDto.UPassword, user.Salt);
                    if (user.UPassword == hashedSaltAndPassword)
                        return GenerateJwt(user.IdUser);
                }

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
