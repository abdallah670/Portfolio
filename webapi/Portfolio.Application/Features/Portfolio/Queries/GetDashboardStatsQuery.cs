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
        var draftProjects = await _context.Projects.CountAsync(p => p.Status == "Draft", cancellationToken);
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
                p.Image
            })
            .ToListAsync(cancellationToken);
        
        _logger.LogInformation(
            "Dashboard stats: Projects={Projects}, Drafts={Drafts}, Messages={Messages}, Unread={Unread}",
            totalProjects, draftProjects, totalMessages, unreadMessages);
        
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
            RecentProjects = recentProjects.Cast<object>().ToList()
        };
    }
}
