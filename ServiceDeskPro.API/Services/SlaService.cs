using ServiceDeskPro.API.Models.Enums;

namespace ServiceDeskPro.API.Services;

public class SlaService
{
    public DateTime CalculateDeadline(IncidentPriority priority)
    {
        var now = DateTime.UtcNow;

        return priority switch
        {
            IncidentPriority.Critical => now.AddHours(2),
            IncidentPriority.High => now.AddHours(8),
            IncidentPriority.Medium => now.AddHours(24),
            IncidentPriority.Low => now.AddHours(48),
            _ => now.AddHours(24)
        };
    }
}