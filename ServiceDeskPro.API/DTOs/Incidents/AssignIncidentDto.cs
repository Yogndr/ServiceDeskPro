using System.ComponentModel.DataAnnotations;

namespace ServiceDeskPro.API.DTOs.Incidents;

public class AssignIncidentDto
{
    [Required]
    public int EngineerId { get; set; }
}