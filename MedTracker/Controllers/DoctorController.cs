using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedTracker.Models;
using MedTracker;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using MedTracker.DTOs;

[Route("[controller]")]
[ApiController]
//[Authorize]
public class DoctorController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DoctorController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("GetTreatments")]
    [Authorize]
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
            if (!(user.Count == 1 && user[0].IsDoctor))
                return BadRequest(new { Message = "The user is not a doctor." });
        }
        catch
        {
            return BadRequest(new { Message = "No user id was found, check the token." });
        }

        try
        {
            var treatments = await _context.Treatments
                .Where(t => t.DoctorID == currentUserId)
                .Select(t => new
                {
                    t.IdTreatment,
                    t.TName,
                    t.StatusTreatment,
                    t.Start_Time,
                    t.End_Time,
                    t.NotePatient,
                    t.NoteDoctor,
                    t.DoctorID,
                })
                .ToListAsync();

            var treatmentsIds = treatments.Select(t => t.IdTreatment).ToList();
            
            var patients = await _context.Patient_Treatment
                .Where(tm => treatmentsIds.Contains(tm.IdTreatment))
                .Join(
                    _context.Patients,
                    tm => tm.IdUser,
                    p => p.IdUser,
                    (tm, p) => new
                    {
                        tm.IdTreatment,
                        p.IdUser,
                        p.FirstName,
                        p.LastName,
                        p.PhoneNumber
                    })
                .ToListAsync();

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

            var treatmentsWithPatientsAndMedications = new List<object>();

            foreach (var treatment in treatments)
            {
                var treatmentId = treatment.IdTreatment;
                var patient = patients.FirstOrDefault(p => p.IdTreatment == treatmentId);
                var meds = medications.Where(m => m.IdTreatment == treatmentId).ToList();

                var treatmentWithPatientAndMedications = new
                {
                    treatment.IdTreatment,
                    treatment.TName,
                    treatment.StatusTreatment,
                    treatment.Start_Time,
                    treatment.End_Time,
                    treatment.NotePatient,
                    treatment.NoteDoctor,
                    treatment.DoctorID,
                    Patient = patient,
                    Medications = meds
                };

                treatmentsWithPatientsAndMedications.Add(treatmentWithPatientAndMedications);
            }

            return Ok(treatmentsWithPatientsAndMedications);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

    [HttpPost("UpdateTreatmentStatus")]
    [Authorize]
    public async Task<IActionResult> UpdateTreatmentStatus([FromBody] UpdateTreatmentStatusDto updateTreatmentStatusDto)
    {
        int currentUserId = 0;
        try
        {
            var claimUserId = User.Claims.First(claim => claim.Type == "userId").Value;
            currentUserId = Convert.ToInt32(claimUserId);

            var user = await _context.Users
                .Where(u => u.IdUser == currentUserId)
                .ToListAsync();
            if (!(user.Count == 1 && user[0].IsDoctor))
                return BadRequest(new { Message = "The user is not a doctor." });
        }
        catch
        {
            return BadRequest(new { Message = "No user id was found, check the token." });
        }

        var treatment = _context.Treatments.Where(t => t.IdTreatment == updateTreatmentStatusDto.IdTreatment).FirstOrDefault();
        
        if (treatment == null)
            return StatusCode(404);

        if (treatment.DoctorID != currentUserId)
            return StatusCode(401);

        treatment.StatusTreatment = updateTreatmentStatusDto.Status;
        treatment.NoteDoctor = updateTreatmentStatusDto.Note;

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
}
