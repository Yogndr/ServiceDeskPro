using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceDeskPro.API.Data;
using ServiceDeskPro.API.DTOs.Incidents;
using ServiceDeskPro.API.Models;
using ServiceDeskPro.API.Models.Enums;
using ServiceDeskPro.API.Services;

namespace ServiceDeskPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IncidentsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly SlaService _slaService;

    public IncidentsController(
        AppDbContext context,
        SlaService slaService)
    {
        _context = context;
        _slaService = slaService;
    }

    // -------------------------------------------------------
    // EMPLOYEE: Create a new incident
    // POST /api/incidents
    // -------------------------------------------------------

    [HttpPost]
    public async Task<IActionResult> CreateIncident(
        CreateIncidentDto request)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var now = DateTime.UtcNow;

        var incident = new Incident
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Category = request.Category.Trim(),
            Priority = request.Priority,

            Status = IncidentStatus.Open,

            CreatedAt = now,
            CreatedById = userId,

            SlaDeadline =
                _slaService.CalculateDeadline(request.Priority)
        };

        _context.Incidents.Add(incident);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetIncidentById),
            new { id = incident.Id },
            new
            {
                incident.Id,
                incident.Title,
                incident.Description,
                incident.Category,

                Priority = incident.Priority.ToString(),
                Status = incident.Status.ToString(),

                incident.CreatedAt,
                incident.SlaDeadline
            });
    }


    // -------------------------------------------------------
    // AUTHENTICATED USER: Get an incident by ID
    // GET /api/incidents/5
    // -------------------------------------------------------

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetIncidentById(int id)
    {
        var incident = await _context.Incidents
            .AsNoTracking()
            .Include(i => i.CreatedBy)
            .Include(i => i.AssignedEngineer)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (incident is null)
        {
            return NotFound(new
            {
                message = "Incident not found."
            });
        }

        return Ok(new
        {
            incident.Id,
            incident.Title,
            incident.Description,
            incident.Category,

            Priority = incident.Priority.ToString(),
            Status = incident.Status.ToString(),

            incident.CreatedAt,
            incident.SlaDeadline,
            incident.ResolvedAt,

            IsSlaBreached =
                incident.Status != IncidentStatus.Resolved &&
                incident.Status != IncidentStatus.Closed &&
                DateTime.UtcNow > incident.SlaDeadline,

            CreatedBy = new
            {
                incident.CreatedBy.Id,
                incident.CreatedBy.Name
            },

            AssignedEngineer =
                incident.AssignedEngineer == null
                    ? null
                    : new
                    {
                        incident.AssignedEngineer.Id,
                        incident.AssignedEngineer.Name
                    }
        });
    }


    // -------------------------------------------------------
    // EMPLOYEE: Get incidents created by logged-in user
    // GET /api/incidents/my
    // -------------------------------------------------------

    [HttpGet("my")]
    public async Task<IActionResult> GetMyIncidents()
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var incidents = await _context.Incidents
            .AsNoTracking()
            .Where(i => i.CreatedById == userId)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new
            {
                i.Id,
                i.Title,
                i.Category,

                Priority = i.Priority.ToString(),
                Status = i.Status.ToString(),

                i.CreatedAt,
                i.SlaDeadline,

                IsSlaBreached =
                    i.Status != IncidentStatus.Resolved &&
                    i.Status != IncidentStatus.Closed &&
                    DateTime.UtcNow > i.SlaDeadline
            })
            .ToListAsync();

        return Ok(incidents);
    }


    // -------------------------------------------------------
    // ADMIN: Get every incident
    // GET /api/incidents
    // -------------------------------------------------------

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllIncidents()
    {
        var incidents = await _context.Incidents
            .AsNoTracking()
            .Include(i => i.CreatedBy)
            .Include(i => i.AssignedEngineer)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new
            {
                i.Id,
                i.Title,
                i.Category,

                Priority = i.Priority.ToString(),
                Status = i.Status.ToString(),

                CreatedBy = i.CreatedBy.Name,

                AssignedEngineer =
                    i.AssignedEngineer == null
                        ? null
                        : i.AssignedEngineer.Name,

                i.CreatedAt,
                i.SlaDeadline,

                IsSlaBreached =
                    i.Status != IncidentStatus.Resolved &&
                    i.Status != IncidentStatus.Closed &&
                    DateTime.UtcNow > i.SlaDeadline
            })
            .ToListAsync();

        return Ok(incidents);
    }


    // -------------------------------------------------------
    // ADMIN: Assign an incident to an engineer
    // PUT /api/incidents/5/assign
    // -------------------------------------------------------

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}/assign")]
    public async Task<IActionResult> AssignEngineer(
        int id,
        AssignIncidentDto request)
    {
        var incident =
            await _context.Incidents.FindAsync(id);

        if (incident is null)
        {
            return NotFound(new
            {
                message = "Incident not found."
            });
        }

        var engineer = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Id == request.EngineerId &&
                u.Role == UserRole.Engineer);

        if (engineer is null)
        {
            return BadRequest(new
            {
                message = "A valid engineer is required."
            });
        }

        if (incident.Status == IncidentStatus.Resolved ||
            incident.Status == IncidentStatus.Closed)
        {
            return BadRequest(new
            {
                message =
                    "Resolved or closed incidents cannot be assigned."
            });
        }

        incident.AssignedEngineerId = engineer.Id;
        incident.Status = IncidentStatus.Assigned;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Engineer assigned successfully.",

            incidentId = incident.Id,

            engineerId = engineer.Id,
            engineerName = engineer.Name,

            status = incident.Status.ToString()
        });
    }


    // -------------------------------------------------------
    // ENGINEER: Get incidents assigned to logged-in engineer
    // GET /api/incidents/assigned
    // -------------------------------------------------------

    [Authorize(Roles = "Engineer")]
    [HttpGet("assigned")]
    public async Task<IActionResult> GetAssignedIncidents()
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var engineerId))
        {
            return Unauthorized();
        }

        var incidents = await _context.Incidents
            .AsNoTracking()
            .Where(i =>
                i.AssignedEngineerId == engineerId)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new
            {
                i.Id,
                i.Title,
                i.Description,
                i.Category,

                Priority = i.Priority.ToString(),
                Status = i.Status.ToString(),

                i.CreatedAt,
                i.SlaDeadline,

                IsSlaBreached =
                    i.Status != IncidentStatus.Resolved &&
                    i.Status != IncidentStatus.Closed &&
                    DateTime.UtcNow > i.SlaDeadline
            })
            .ToListAsync();

        return Ok(incidents);
    }


    // -------------------------------------------------------
    // ENGINEER: Change status of assigned incident
    // PUT /api/incidents/5/status
    // -------------------------------------------------------

    [Authorize(Roles = "Engineer")]
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        UpdateIncidentStatusDto request)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var engineerId))
        {
            return Unauthorized();
        }

        var incident =
            await _context.Incidents.FindAsync(id);

        if (incident is null)
        {
            return NotFound(new
            {
                message = "Incident not found."
            });
        }

        // An engineer can modify only incidents assigned to them.
        if (incident.AssignedEngineerId != engineerId)
        {
            return Forbid();
        }

        // Allowed:
        // Assigned -> InProgress
        // InProgress -> Resolved

        var validTransition =
            (incident.Status == IncidentStatus.Assigned &&
             request.Status == IncidentStatus.InProgress)
            ||
            (incident.Status == IncidentStatus.InProgress &&
             request.Status == IncidentStatus.Resolved);

        if (!validTransition)
        {
            return BadRequest(new
            {
                message =
                    $"Invalid status transition from " +
                    $"{incident.Status} to {request.Status}."
            });
        }

        incident.Status = request.Status;

        if (request.Status == IncidentStatus.Resolved)
        {
            incident.ResolvedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Incident status updated.",

            incident.Id,

            status = incident.Status.ToString(),

            incident.ResolvedAt
        });
    }
}