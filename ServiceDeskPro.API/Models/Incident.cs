using ServiceDeskPro.API.Models.Enums;

namespace ServiceDeskPro.API.Models;

public class Incident
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public IncidentPriority Priority { get; set; } = IncidentPriority.Medium;

    public IncidentStatus Status { get; set; } = IncidentStatus.Open;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ResolvedAt { get; set; }

    public DateTime SlaDeadline { get; set; }


    // Employee who reported the incident
    public int CreatedById { get; set; }

    public User CreatedBy { get; set; } = null!;


    // Engineer assigned to it — null until somebody is assigned
    public int? AssignedEngineerId { get; set; }

    public User? AssignedEngineer { get; set; }


    public ICollection<Comment> Comments { get; set; }
        = new List<Comment>();
}