using System;
using System.Collections.Generic;

namespace PortfolioApi.Domain.Entities;

public class VisitorSession
{
    public int Id { get; set; }
    public string SessionId { get; set; } = Guid.NewGuid().ToString("N");
    public string HashedIpAddress { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty; // Mobile/Desktop/Tablet
    public string Browser { get; set; } = string.Empty;
    public string OperatingSystem { get; set; } = string.Empty;
    public string ScreenResolution { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }
    public bool IsBounced { get; set; } = true;
    public int TotalPageViews { get; set; } = 0;
    
    public ICollection<PageVisit> PageVisits { get; set; } = new List<PageVisit>();
}
