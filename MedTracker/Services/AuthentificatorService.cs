using MedTracker.DTOs;
using MedTracker.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace MedTracker.Services
{
    public class AuthentificatorService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

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
                var user = new User
                {
                    Email = requestDto.Email,
                    UPassword = requestDto.UPassword, // Note: You should hash and salt the password before storing it
                };
                var patient = new Patient
                {
                    FirstName = requestDto.FirstName,
                    LastName = requestDto.LastName,
                    PhoneNumber = requestDto.PhoneNumber,
                    DateofBirth = requestDto.DateofBirth,
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

        public LoginResultDto LoginUser(LoginRequestDto requestDto)
        {
            try
            {
                // Find the user by email
                var user = _dbContext.Users.FirstOrDefault(u => u.Email == requestDto.Email);

                if (user == null)
                {
                    return new LoginResultDto
                    {
                        Success = false,
                        Message = "User not found."
                    };
                }

                // Check the password (You should implement password hashing and verification logic here)
                if (user.UPassword == requestDto.UPassword)
                {
                    return new LoginResultDto
                    {
                        Success = true,
                        Message = "Login successful"
                        // You can include additional user information here if needed
                        // User = user
                    };
                }
                else
                {
                    return new LoginResultDto
                    {
                        Success = false,
                        Message = "Incorrect password."
                    };
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"An error occurred during login: {ex.Message}");

                return new LoginResultDto
                {
                    Success = false,
                    Message = "An error occurred during login."
                };
            }
        }

        private bool IsEmailTaken(string email)
        {
            return _dbContext.Users.Any(u => u.Email == email);
        }

    }
}
