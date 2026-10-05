using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceDeskPro.API.Data;
using ServiceDeskPro.API.Models.Enums;

namespace ServiceDeskPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }


    // -------------------------------------------------------
    // ADMIN: Dashboard statistics
    // GET /api/dashboard/stats
    // -------------------------------------------------------

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var now = DateTime.UtcNow;

        var total = await _context.Incidents.CountAsync();

        var open = await _context.Incidents
            .CountAsync(i => i.Status == IncidentStatus.Open);

        var assigned = await _context.Incidents
            .CountAsync(i => i.Status == IncidentStatus.Assigned);

        var inProgress = await _context.Incidents
            .CountAsync(i => i.Status == IncidentStatus.InProgress);

        var resolved = await _context.Incidents
            .CountAsync(i => i.Status == IncidentStatus.Resolved);

        var closed = await _context.Incidents
            .CountAsync(i => i.Status == IncidentStatus.Closed);

        var slaBreached = await _context.Incidents
            .CountAsync(i =>
                i.Status != IncidentStatus.Resolved &&
                i.Status != IncidentStatus.Closed &&
                now > i.SlaDeadline);

        return Ok(new
        {
            total,
            open,
            assigned,
            inProgress,
            resolved,
            closed,
            slaBreached
        });
    }


    // -------------------------------------------------------
    // ADMIN: Filter incidents
    //
    // Examples:
    // GET /api/dashboard/incidents?status=Open
    // GET /api/dashboard/incidents?priority=High
    // GET /api/dashboard/incidents?category=Network
    // -------------------------------------------------------

    [HttpGet("incidents")]
    public async Task<IActionResult> FilterIncidents(
        IncidentStatus? status,
        IncidentPriority? priority,
        string? category)
    {
        var query = _context.Incidents
            .AsNoTracking()
            .Include(i => i.CreatedBy)
            .Include(i => i.AssignedEngineer)
            .AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(i => i.Status == status.Value);
        }

        if (priority.HasValue)
        {
            query = query.Where(i => i.Priority == priority.Value);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(i =>
                i.Category.ToLower() == category.Trim().ToLower());
        }

        var now = DateTime.UtcNow;

        var incidents = await query
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
                    now > i.SlaDeadline
            })
            .ToListAsync();

        return Ok(incidents);
    }
}