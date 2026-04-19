using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Portfolio.Queries;

public class GetDashboardStatsQuery : IRequest<DashboardStatsDto>
{
}

public class DashboardStatsDto
{
    public int TotalProjects { get; set; }
    public int DraftProjects { get; set; }
    public int TotalMessages { get; set; }
    public int UnreadMessages { get; set; }
    public int RepliedMessages { get; set; }
    public int TotalSkills { get; set; }
    public int SkillCategories { get; set; }
    public int ProfileViews { get; set; }
    public List<object> RecentProjects { get; set; } = new();
    public List<MonthlyStatDto> ProjectsByMonth { get; set; } = new();
    public List<MonthlyStatDto> MessagesByMonth { get; set; } = new();
    public List<ProjectViewsDto> ViewsByMonth { get; set; } = new();
}

public class MonthlyStatDto
{
    public string Month { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class ProjectViewsDto
{
    public string Name { get; set; } = string.Empty;
    public int Views { get; set; }
}

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetDashboardStatsQueryHandler> _logger;

    public GetDashboardStatsQueryHandler(IApplicationDbContext context, ILogger<GetDashboardStatsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching dashboard statistics");
        
        var totalProjects = await _context.Projects.CountAsync(cancellationToken);
        var draftProjects = await _context.Projects.CountAsync(p => !p.IsPublished, cancellationToken);
        var totalMessages = await _context.Messages.CountAsync(cancellationToken);
        var unreadMessages = await _context.Messages.CountAsync(m => !m.IsRead, cancellationToken);
        var totalSkills = await _context.Skills.CountAsync(cancellationToken);
        var skillCategories = await _context.SkillCategories.CountAsync(cancellationToken);
        
        // Real project views from ViewsCount
        var profileViews = await _context.Projects.SumAsync(p => p.ViewsCount, cancellationToken);
        
        // Reply counts
        var repliedMessages = await _context.Messages.CountAsync(m => m.IsReplied, cancellationToken);
        
        var recentProjects = await _context.Projects
            .OrderByDescending(p => p.Id)
            .Take(5)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Description,
                p.Stack,
                p.Status,
                p.Image,
                p.Year,
                p.Category
            })
            .ToListAsync(cancellationToken);

        // Monthly stats (last 12 months including current)
        var now = DateTime.UtcNow;
        var twelveMonthsAgo = now.AddMonths(-11);
        
        // Get all projects with their names and view counts
        var projectViews = await _context.Projects
            .Where(p => p.ViewsCount > 0)
            .OrderByDescending(p => p.ViewsCount)
            .Take(10) // Top 10 most viewed projects
            .Select(p => new ProjectViewsDto { Name = p.Title, Views = p.ViewsCount })
            .ToListAsync(cancellationToken);
        
        // Generate last 12 months list
        var months = Enumerable.Range(0, 12)
            .Select(i => {
                var d = now.AddMonths(-i);
                return $"{d.Year}-{d.Month:D2}";
            })
            .Reverse()
            .ToList();
        
        // Monthly Messages stats (last 12 months)
        var messageMonthlyRaw = await _context.Messages
            .Where(m => m.CreatedAt >= twelveMonthsAgo)
            .Select(m => new { m.CreatedAt.Year, m.CreatedAt.Month })
            .ToListAsync(cancellationToken);
        
        var messageMonthly = months.Select(month => {
            var count = messageMonthlyRaw.Count(m => $"{m.Year}-{m.Month:D2}" == month);
            return new MonthlyStatDto { Month = month, Count = count };
        }).ToList();
        
        _logger.LogInformation(
            "Dashboard stats: Projects={Projects}, Drafts={Drafts}, Messages={Messages}, Unread={Unread}, Replied={Replied}, Skills={Skills}, Categories={Categories}, Views={Views}",
            totalProjects, draftProjects, totalMessages, unreadMessages, repliedMessages, totalSkills, skillCategories, profileViews);
        
        return new DashboardStatsDto
        {
            TotalProjects = totalProjects,
            DraftProjects = draftProjects,
            TotalMessages = totalMessages,
            UnreadMessages = unreadMessages,
            RepliedMessages = repliedMessages,
            TotalSkills = totalSkills,
            SkillCategories = skillCategories,
            ProfileViews = profileViews,
            RecentProjects = recentProjects.Cast<object>().ToList(),
            ProjectsByMonth = months.Select(m => new MonthlyStatDto { Month = m, Count = 0 }).ToList(),
            MessagesByMonth = messageMonthly,
            ViewsByMonth = projectViews
        };
    }
}
