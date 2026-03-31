using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Analytics.Queries;

public record GetAnalyticsDashboardQuery(int Days) : IRequest<DashboardAnalyticsDto>;

public class GetAnalyticsDashboardQueryHandler : IRequestHandler<GetAnalyticsDashboardQuery, DashboardAnalyticsDto>
{
    private readonly IApplicationDbContext _context;

    public GetAnalyticsDashboardQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardAnalyticsDto> Handle(GetAnalyticsDashboardQuery request, CancellationToken cancellationToken)
    {
        var startDate = DateTime.UtcNow.AddDays(-request.Days);
        
        // Get total visitors (unique sessions)
        var totalVisitors = await _context.VisitorSessions
            .Where(s => s.StartedAt >= startDate)
            .CountAsync(cancellationToken);
        
        // Get total page views
        var totalPageViews = await _context.PageVisits
            .Where(v => v.VisitedAt >= startDate)
            .CountAsync(cancellationToken);
        
        // Calculate bounce rate
        var totalSessions = await _context.VisitorSessions
            .Where(s => s.StartedAt >= startDate)
            .CountAsync(cancellationToken);
        
        var bouncedSessions = await _context.VisitorSessions
            .Where(s => s.StartedAt >= startDate && s.IsBounced)
            .CountAsync(cancellationToken);
        
        var bounceRate = totalSessions > 0 ? (double)bouncedSessions / totalSessions * 100 : 0;
        
        // Calculate average session duration
        var sessionsWithDuration = await _context.VisitorSessions
            .Where(s => s.StartedAt >= startDate && s.EndedAt.HasValue)
            .Select(s => (s.EndedAt.Value - s.StartedAt).TotalSeconds)
            .ToListAsync(cancellationToken);
        
        var avgSessionDuration = sessionsWithDuration.Any() 
            ? TimeSpan.FromSeconds(sessionsWithDuration.Average()) 
            : TimeSpan.Zero;
        
        // Get traffic trends (daily)
        var trafficTrends = await GetTrafficTrendsAsync(startDate, cancellationToken);
        
        // Get top projects
        var topProjects = await GetTopProjectsAsync(startDate, cancellationToken);
        
        // Get top locations
        var topLocations = await GetTopLocationsAsync(startDate, cancellationToken);
        
        // Get device breakdown
        var deviceBreakdown = await GetDeviceBreakdownAsync(startDate, cancellationToken);
        
        return new DashboardAnalyticsDto
        {
            TotalVisitors = totalVisitors,
            TotalPageViews = totalPageViews,
            BounceRate = Math.Round(bounceRate, 1),
            AverageSessionDuration = avgSessionDuration,
            TrafficTrends = trafficTrends,
            TopProjects = topProjects,
            TopLocations = topLocations,
            DeviceBreakdown = deviceBreakdown
        };
    }
    
    private async Task<List<TrafficTrendsDto>> GetTrafficTrendsAsync(DateTime startDate, CancellationToken cancellationToken)
    {
        var result = new List<TrafficTrendsDto>();
        
        // Group by date and get counts
        var dailyData = await _context.PageVisits
            .Where(v => v.VisitedAt >= startDate)
            .GroupBy(v => v.VisitedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                PageViews = g.Count(),
                Visitors = g.Select(v => v.SessionId).Distinct().Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);
        
        foreach (var data in dailyData)
        {
            result.Add(new TrafficTrendsDto
            {
                Date = data.Date,
                Visitors = data.Visitors,
                PageViews = data.PageViews
            });
        }
        
        return result;
    }
    
    private async Task<List<ProjectViewsDto>> GetTopProjectsAsync(DateTime startDate, CancellationToken cancellationToken)
    {
        var projectViews = await _context.PageVisits
            .Where(v => v.VisitedAt >= startDate && v.ProjectId.HasValue)
            .GroupBy(v => v.ProjectId.Value)
            .Select(g => new { ProjectId = g.Key, Views = g.Count() })
            .OrderByDescending(x => x.Views)
            .Take(10)
            .ToListAsync(cancellationToken);
        
        if (!projectViews.Any()) return new List<ProjectViewsDto>();
        
        var projectIds = projectViews.Select(p => p.ProjectId).ToList();
        var projects = await _context.Projects
            .Where(p => projectIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Title, cancellationToken);
        
        var maxViews = projectViews.Max(p => p.Views);
        
        return projectViews.Select(p => new ProjectViewsDto
        {
            ProjectId = p.ProjectId,
            ProjectName = projects.GetValueOrDefault(p.ProjectId, $"Project {p.ProjectId}"),
            Views = p.Views,
            Percentage = maxViews > 0 ? (double)p.Views / maxViews * 100 : 0
        }).ToList();
    }
    
    private async Task<List<GeoLocationDto>> GetTopLocationsAsync(DateTime startDate, CancellationToken cancellationToken)
    {
        var locations = await _context.VisitorSessions
            .Where(s => s.StartedAt >= startDate && !string.IsNullOrEmpty(s.Country))
            .GroupBy(s => s.Country)
            .Select(g => new { Country = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync(cancellationToken);
        
        if (!locations.Any()) return new List<GeoLocationDto>();
        
        var total = locations.Sum(l => l.Count);
        
        return locations.Select(l => new GeoLocationDto
        {
            Country = l.Country,
            City = string.Empty,
            VisitorCount = l.Count,
            Percentage = total > 0 ? (double)l.Count / total * 100 : 0
        }).ToList();
    }
    
    private async Task<List<DeviceBreakdownDto>> GetDeviceBreakdownAsync(DateTime startDate, CancellationToken cancellationToken)
    {
        var devices = await _context.VisitorSessions
            .Where(s => s.StartedAt >= startDate)
            .GroupBy(s => s.DeviceType)
            .Select(g => new { DeviceType = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);
        
        if (!devices.Any()) return new List<DeviceBreakdownDto>();
        
        var total = devices.Sum(d => d.Count);
        
        return devices.Select(d => new DeviceBreakdownDto
        {
            DeviceType = d.DeviceType,
            Count = d.Count,
            Percentage = total > 0 ? (double)d.Count / total * 100 : 0
        }).ToList();
    }
}
