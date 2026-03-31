namespace PortfolioApi.Application.DTOs;

public class TrafficTrendsDto
{
    public DateTime Date { get; set; }
    public int Visitors { get; set; }
    public int PageViews { get; set; }
}

public class ProjectViewsDto
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public int Views { get; set; }
    public double Percentage { get; set; }
}

public class GeoLocationDto
{
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public int VisitorCount { get; set; }
    public double Percentage { get; set; }
}

public class DeviceBreakdownDto
{
    public string DeviceType { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class DashboardAnalyticsDto
{
    public int TotalVisitors { get; set; }
    public int TotalPageViews { get; set; }
    public double BounceRate { get; set; }
    public TimeSpan AverageSessionDuration { get; set; }
    public List<TrafficTrendsDto> TrafficTrends { get; set; } = new();
    public List<ProjectViewsDto> TopProjects { get; set; } = new();
    public List<GeoLocationDto> TopLocations { get; set; } = new();
    public List<DeviceBreakdownDto> DeviceBreakdown { get; set; } = new();
}
