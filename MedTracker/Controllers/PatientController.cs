using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedTracker.Models;
using MedTracker.DTOs;
using Microsoft.AspNetCore.Authorization;
using MedTracker;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Net.Sockets;

[Authorize]
[Route("[controller]")]
[ApiController]
public class PatientController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PatientController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("AddTreatment")]
    public async Task<IActionResult> AddTreatment([FromBody] TreatmentCreateDto treatmentDto)
    {
        // Retrieve the current user's ID from HttpContext.Items
        int currentUserId = 0;
        try
        {
            var claimUserId = User.Claims.First(claim => claim.Type == "userId").Value;
            currentUserId = Convert.ToInt32(claimUserId);

            var user = await _context.Users
                .Where(u => u.IdUser == currentUserId)
                .ToListAsync();
            if (!(user.Count == 1 && !user[0].IsDoctor))
                return BadRequest(new { Message = "The user is not a patient." });
        }
        catch
        {
            return BadRequest(new { Message = "No user id was found, check the token." });
        }

        // Create a new Treatment
        var treatment = new Treatment
        {
            TName = treatmentDto.TName,
            Start_Time = treatmentDto.Start_Time,
            End_Time = treatmentDto.End_Time,
            Note = treatmentDto.Note,
            DoctorID = treatmentDto.DoctorID
        };

        try
        {
            // Add the Treatment to the database
            _context.Treatments.Add(treatment);
            await _context.SaveChangesAsync();
        }
        catch
        {
            return BadRequest();
        }

        var patientTreatment = new Patient_Treatment
        {
            IdUser = currentUserId,
            IdTreatment = treatment.IdTreatment
        };

        try
        {
            // Add the Treatment link to Patient in the database
            _context.Patient_Treatment.Add(patientTreatment);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                TreatmentId = treatment.IdTreatment
            });
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpPost("AddMedication")]
    public async Task<IActionResult> AddMedication(AddMedicationDbo medicationDbo)
    {
        // Retrieve the current user's ID from HttpContext.Items
        int currentUserId = 0;
        try
        {
            var claimUserId = User.Claims.First(claim => claim.Type == "userId").Value;
            currentUserId = Convert.ToInt32(claimUserId);

            var user = await _context.Users
                .Where(u => u.IdUser == currentUserId)
                .ToListAsync();
            if (!(user.Count == 1 && !user[0].IsDoctor))
                return BadRequest(new { Message = "The user is not a patient." });
        }
        catch
        {
            return BadRequest(new { Message = "No user id was found, check the token." });
        }

        var medication = new Medication
        {
            PName = medicationDbo.PName,
            MDescription = medicationDbo.MDescription,
            Start_Time = medicationDbo.Start_Time,
            End_Time = medicationDbo.End_Time,
            TimeUse = medicationDbo.TimeUse,
            Quantity = medicationDbo.Quantity
        };

        try
        {
            // Add the Medication to the database
            _context.Medications.Add(medication);
            await _context.SaveChangesAsync();
        }
        catch
        {
            return BadRequest();
        }

        var treatmentMedication = new Treatment_Medication
        {
            IdTreatment = medicationDbo.IdTreatment,
            IdMedication = medication.IdMedication
        };

        try
        {
            // Add the Medictation Link to Treatment in the database
            _context.Treatment_Medication.Add(treatmentMedication);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }

    [HttpGet("GetTreatments")]
    public async Task<IActionResult> GetTreatments()
    {
        int currentUserId = 0;
        try
        {
            var claimUserId = User.Claims.First(claim => claim.Type == "userId").Value;
            currentUserId = Convert.ToInt32(claimUserId);

            var user = await _context.Users
                .Where(u => u.IdUser == currentUserId)
                .ToListAsync();
            if (!(user.Count == 1 && !user[0].IsDoctor))
                return BadRequest(new { Message = "The user is not a patient." });
        }
        catch
        {
            return BadRequest(new { Message = "No user id was found, check the token." });
        }

        try
        {
            // Fetch treatments associated with the current patient
            var treatments = await _context.Patient_Treatment
                .Where(pt => pt.IdUser == currentUserId)
                .Join(
                    _context.Treatments,
                    pt => pt.IdTreatment,
                    t => t.IdTreatment,
                    (pt, t) => new
                    {
                        t.IdTreatment,
                        t.TName,
                        t.Start_Time,
                        t.End_Time,
                        t.Note,
                        t.DoctorID
                    })
                .ToListAsync();

            var treatmentsIds = new List<int>();
            foreach (var treatment in treatments)
            {
                treatmentsIds.Add(treatment.IdTreatment);
            }

            var medications = await _context.Treatment_Medication
                .Where(tm => treatmentsIds.Contains(tm.IdTreatment))
                .Join(
                    _context.Medications,
                    tm => tm.IdMedication,
                    m => m.IdMedication,
                    (tm, m) => new
                    {
                        tm.IdTreatment,
                        m.IdMedication,
                        m.PName,
                        m.MDescription,
                        m.Start_Time,
                        m.End_Time,
                        m.TimeUse,
                        m.Quantity
                    })
                .ToListAsync();

            var treatmentsWithMedications = treatments
            .GroupJoin(
                medications,
                t => t.IdTreatment,
                m => m.IdTreatment,
                (t, meds) => new
                {
                    t.IdTreatment,
                    t.TName,
                    t.Start_Time,
                    t.End_Time,
                    t.Note,
                    t.DoctorID,
                    Medications = meds.ToList()
                })
            .ToList();

            return Ok(treatmentsWithMedications);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred:{ex.Message}");
        }
        
    }
}
