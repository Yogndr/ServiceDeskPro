namespace ServiceDeskPro.API.Models;

public class Comment
{
    public int Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int IncidentId { get; set; }

    public Incident Incident { get; set; } = null!;

    public int UserId { get; set; }

    public User User { get; set; } = null!;
}