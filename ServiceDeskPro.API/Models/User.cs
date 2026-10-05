using ServiceDeskPro.API.Models.Enums;

namespace ServiceDeskPro.API.Models;

public class User
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Employee;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Incident> CreatedIncidents { get; set; }
        = new List<Incident>();

    public ICollection<Incident> AssignedIncidents { get; set; }
        = new List<Incident>();

    public ICollection<Comment> Comments { get; set; }
        = new List<Comment>();
}