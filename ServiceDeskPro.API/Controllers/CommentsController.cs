using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceDeskPro.API.Data;
using ServiceDeskPro.API.DTOs.Comments;
using ServiceDeskPro.API.Models;

namespace ServiceDeskPro.API.Controllers;

[ApiController]
[Route("api/incidents/{incidentId:int}/comments")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CommentsController(AppDbContext context)
    {
        _context = context;
    }


    // Add comment to an incident
    // POST /api/incidents/1/comments

    [HttpPost]
    public async Task<IActionResult> AddComment(
        int incidentId,
        AddCommentDto request)
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var incidentExists = await _context.Incidents
            .AnyAsync(i => i.Id == incidentId);

        if (!incidentExists)
        {
            return NotFound(new
            {
                message = "Incident not found."
            });
        }

        var comment = new Comment
        {
            Content = request.Content.Trim(),
            IncidentId = incidentId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Comment added successfully.",
            comment = new
            {
                comment.Id,
                comment.Content,
                comment.IncidentId,
                comment.UserId,
                comment.CreatedAt
            }
        });
    }


    // Get comments for an incident
    // GET /api/incidents/1/comments

    [HttpGet]
    public async Task<IActionResult> GetComments(int incidentId)
    {
        var incidentExists = await _context.Incidents
            .AnyAsync(i => i.Id == incidentId);

        if (!incidentExists)
        {
            return NotFound(new
            {
                message = "Incident not found."
            });
        }

        var comments = await _context.Comments
            .AsNoTracking()
            .Where(c => c.IncidentId == incidentId)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.Content,
                c.CreatedAt,

                User = new
                {
                    c.User.Id,
                    c.User.Name,
                    Role = c.User.Role.ToString()
                }
            })
            .ToListAsync();

        return Ok(comments);
    }
}