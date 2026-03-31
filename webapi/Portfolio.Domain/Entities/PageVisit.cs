using System;

namespace PortfolioApi.Domain.Entities;

public class PageVisit
{
    public int Id { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int? ProjectId { get; set; }
    public DateTime VisitedAt { get; set; } = DateTime.UtcNow;
    public int TimeOnPage { get; set; } = 0; // Seconds
    public string Referrer { get; set; } = string.Empty;
    
    public VisitorSession Session { get; set; } = null!;
}
