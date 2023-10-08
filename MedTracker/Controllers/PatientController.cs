using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedTracker.Models;
using MedTracker.DTOs;
using Microsoft.AspNetCore.Authorization;
using MedTracker;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

[Route("api/[controller]")]
[ApiController]
public class PatientController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PatientController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/Patient/AddTreatment
    [HttpPost("AddTreatment")]
    //[Authorize]
    public async Task<IActionResult> AddTreatment([FromBody] TreatmentCreateDto treatmentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int currentUserId))
        {
            return BadRequest("Invalid user ID");
        }

        // Create a new Treatment
        var treatment = new Treatment
        {
            TName = treatmentDto.TName,
            Start = treatmentDto.Start,
            End = treatmentDto.End,
            Note = treatmentDto.Note,
            IdUserD = currentUserId // Assuming this is how you associate the patient with the treatment
        };

        // Add Medications to the Treatment
        foreach (var medicationId in treatmentDto.MedicationIds)
        {
            var medication = await _context.Medications.FindAsync(medicationId);
            if (medication != null)
            {
                var treatmentMedication = new Treatment_Medication
                {
                    IdTreatment = treatment.IdTreatment,
                    IdMedication = medicationId
                };
                _context.Treatment_Medications.Add(treatmentMedication);
            }
        }

        // Add the Treatment to the database
        _context.Treatments.Add(treatment);
        await _context.SaveChangesAsync();

        return Ok(treatment);
    }

    // GET: api/Patient/GetTreatments
    [HttpGet("GetTreatments")]
    //[Authorize]
    public async Task<IActionResult> GetTreatments()
    {
        string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int currentUserId))
        {
            return BadRequest("Invalid user ID");
        }

        //string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //if (!int.TryParse(userIdString, out int currentUserId))
        //{
        //    return BadRequest("Invalid user ID");
        //}
        int currentUserId = 3;
        var treatments = await _context.Patient_Treatments
            .Where(pt => pt.IdUser == currentUserId)
            .Join(_context.Treatments,
                pt => pt.IdTreatment,
                t => t.IdTreatment,
                (pt, t) => t)
            .ToListAsync();

        return Ok(treatments);
        //string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //if (!int.TryParse(userIdString, out int currentUserId))
        //{
        //    return BadRequest("Invalid user ID");
        //}

        /*int currentUserId = 3;

        var treatmentsId = await _context.Patient_Treatments
            .Where(p => p.IdUser == currentUserId)
            .Select(p => p.IdTreatment)
            .ToListAsync();

        List<Treatment> treatments = new();
        foreach (var treatmentId in treatmentsId)
        {
            treatments.AddRange(_context.Treatments
                .Where(predicate: t => t.IdTreatment == treatmentId));
        }
        //var treatments = await _context.Treatments
        //    .Where(t => t.IdUserD == currentUserId) // Assuming this is how treatments are associated with patients
        //    .ToListAsync();

        return Ok(treatments);*/
    }
}
