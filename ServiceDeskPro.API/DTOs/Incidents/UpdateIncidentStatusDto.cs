using ServiceDeskPro.API.Models.Enums;

namespace ServiceDeskPro.API.DTOs.Incidents;

public class UpdateIncidentStatusDto
{
    public IncidentStatus Status { get; set; }
}