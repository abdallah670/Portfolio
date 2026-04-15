using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Portfolio.Queries;

public class GetAllProjectsAdminQuery : IRequest<List<object>>
{
}

public class GetAllProjectsAdminQueryHandler : IRequestHandler<GetAllProjectsAdminQuery, List<object>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetAllProjectsAdminQueryHandler> _logger;

    public GetAllProjectsAdminQueryHandler(IApplicationDbContext context, ILogger<GetAllProjectsAdminQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<object>> Handle(GetAllProjectsAdminQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin fetching all projects including drafts");
        
        var projects = await _context.Projects
            .OrderByDescending(p => p.IsFeatured)
            .ThenBy(p => p.DisplayOrder)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Description,
                p.Stack,
                p.Status,
                p.Image,
                p.Year,
                p.Category,
                p.linkedinUrl,
                p.GithubUrl,
                p.Color,
                p.IsFeatured,
                p.IsPublished,
                p.DisplayOrder,
                p.ViewsCount
            })
            .ToListAsync(cancellationToken);
        
        _logger.LogInformation("Admin retrieved {Count} projects", projects.Count);
        return projects.Cast<object>().ToList();
    }
}
