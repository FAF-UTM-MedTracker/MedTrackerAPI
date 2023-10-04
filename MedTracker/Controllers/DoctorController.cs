using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedTracker.Models;
using MedTracker;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
           // Fetch treatments associated with the current patient
           var treatments = await _context.Treatments
               .Where(t => t.DoctorID == currentUserId)
               .Select(t => new
               {
                   t.IdTreatment,
                   t.TName,
                   t.Start_Time,
                   t.End_Time,
                   t.Note,
                   t.DoctorID,
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
