using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceDeskPro.API.Data;
using ServiceDeskPro.API.Models.Enums;

namespace ServiceDeskPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("engineers")]
    public async Task<IActionResult> GetEngineers()
    {
        var engineers = await _context.Users
            .Where(u => u.Role == UserRole.Engineer)
            .Select(u => new
            {
                u.Id,
                u.Name,
                u.Email
            })
            .ToListAsync();

        return Ok(engineers);
    }
}