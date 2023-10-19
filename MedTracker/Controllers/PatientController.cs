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
                        t.StatusTreatment,
                        t.Start_Time,
                        t.End_Time,
                        t.NoteDoctor,
                        t.NotePatient,
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
                    t.StatusTreatment,
                    t.Start_Time,
                    t.End_Time,
                    t.NotePatient,
                    t.NoteDoctor,
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

    [HttpGet("GetDoctors")]
    public async Task<IActionResult> GetDoctors()
    {
        try
        {
            // Fetch doctors and their information
            var doctors = await _context.Doctors
                .Select(d => new
                {   
                    DocID = d.IdUser,
                    FName = d.FirstName,
                    LName = d.LastName,
                    Email = d.User.Email,
                    PhoneNumber = d.PhoneNumber
                })
                .ToListAsync();

            return Ok(doctors);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
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
            NotePatient = treatmentDto.Note,
            DoctorID = treatmentDto.DoctorID,
            StatusTreatment = "pending"
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

    [HttpPost("RemoveTreatment")]
    public async Task<IActionResult> RemoveTreatment(RemoveTreatmentDto removeTreatmentDto)
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

        var treatment = _context.Treatments.Where(treatment => treatment.IdTreatment == removeTreatmentDto.IdTreatment).FirstOrDefault();

        if (treatment == null)
        {
            return StatusCode(404);
        }

        var treatmentMedications = _context.Treatment_Medication.Where(tm => tm.IdTreatment == removeTreatmentDto.IdTreatment);
        var patientTreatments = _context.Patient_Treatment.Where(pt => pt.IdTreatment == removeTreatmentDto.IdTreatment);

        // We should check if the requested treatment is owned by the caller, but leave it right now
        // We should also remove the medications for this treatment

        try
        {
            _context.Patient_Treatment.RemoveRange(patientTreatments);
            _context.Treatment_Medication.RemoveRange(treatmentMedications);
            _context.Treatments.Remove(treatment);
            await _context.SaveChangesAsync();
        }
        catch
        {
            return BadRequest();
        }

        return StatusCode(200);
    }

    [HttpPost("UpdateTreatment")]
    [Authorize]
    public async Task<IActionResult> UpdateTreatment(UpdateTreatmentDto updateTreatmentDto)
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

        var treatment = _context.Treatments.Where(treatment => treatment.IdTreatment == updateTreatmentDto.TreatmentId).FirstOrDefault();

        if (treatment == null)
            return StatusCode(404);

        var patientTreatment = _context.Patient_Treatment.Where(
            pt => pt.IdUser == currentUserId && pt.IdTreatment == treatment.IdTreatment).FirstOrDefault();

        if (patientTreatment == null)
            return StatusCode(401);

        treatment.TName = updateTreatmentDto.Name ?? treatment.TName;
        treatment.Start_Time = updateTreatmentDto.StartTime ?? treatment.Start_Time;
        treatment.End_Time = updateTreatmentDto.EndTime ?? treatment.End_Time;
        treatment.NotePatient = updateTreatmentDto.Note ?? treatment.NotePatient;
        treatment.DoctorID = updateTreatmentDto.DoctorId ?? treatment.DoctorID;

        try
        {
            _context.Treatments.Update(treatment);
            await _context.SaveChangesAsync();
        }
        catch
        {
            return BadRequest();
        }

        return StatusCode(200);
    }

    [HttpPost("AddMedication")]
    public async Task<IActionResult> AddMedication(AddMedicationDto medicationDbo)
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

    [HttpPost("RemoveMedication")]
    public async Task<IActionResult> RemoveMedication(RemoveMedicationDto removeMedicationDbo)
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

        var medication = _context.Medications.Where(medication => medication.IdMedication == removeMedicationDbo.IdMedication).FirstOrDefault();

        if (medication == null)
        {
            return StatusCode(404);
        }

        var treatmentMedications = _context.Treatment_Medication.Where(tm => tm.IdMedication == removeMedicationDbo.IdMedication);
        
        // We should check if the requested medication is owned by the caller, but leave it right now

        try
        {
            _context.Treatment_Medication.RemoveRange(treatmentMedications);
            _context.Medications.Remove(medication);
            await _context.SaveChangesAsync();
        }
        catch
        {
            return BadRequest();
        }

        return StatusCode(200);
    }

    [HttpPost("UpdateMedication")]
    [Authorize]
    public async Task<IActionResult> UpdateMedication(UpdateMedicationDto updateMedicationDto)
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

        var medication = _context.Medications.Where(medication => medication.IdMedication == updateMedicationDto.IdMedication).FirstOrDefault();

        if (medication == null)
            return StatusCode(404);

        // We should check if the requested medication is owned by the caller, but leave it right now

        medication.PName = updateMedicationDto.Name ?? medication.PName;
        medication.MDescription = updateMedicationDto.Description ?? medication.MDescription;
        medication.Start_Time = updateMedicationDto.StartTime ?? medication.Start_Time;
        medication.End_Time = updateMedicationDto.EndTime ?? medication.End_Time;
        medication.TimeUse = updateMedicationDto.TimeUse ?? medication.TimeUse;
        medication.Quantity = updateMedicationDto.Quantity ?? medication.Quantity;

        try
        {
            _context.Medications.Update(medication);
            await _context.SaveChangesAsync();
        }
        catch
        {
            return BadRequest();
        }

        return StatusCode(200);
    }
    
}
