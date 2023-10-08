using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedTracker.Models;
using MedTracker;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class DoctorController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DoctorController(ApplicationDbContext context)
    {
        _context = context;
    }


    // GET: api/Patient/GetTreatments
    [HttpGet("GetTreatments")]
    [Authorize]
    public async Task<IActionResult> GetTreatments()
    {
        string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out int currentUserId))
        {
            return BadRequest("Invalid user ID");
        }

        var treatments = await _context.Treatments
            .Where(t => t.IdUserD == currentUserId)
            .ToListAsync();

        return Ok(treatments);
    }
}
