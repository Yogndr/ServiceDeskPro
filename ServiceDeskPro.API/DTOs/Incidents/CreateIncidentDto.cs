using System.ComponentModel.DataAnnotations;
using ServiceDeskPro.API.Models.Enums;

namespace ServiceDeskPro.API.DTOs.Incidents;

public class CreateIncidentDto
{
    [Required]
    [MaxLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    public IncidentPriority Priority { get; set; }
}