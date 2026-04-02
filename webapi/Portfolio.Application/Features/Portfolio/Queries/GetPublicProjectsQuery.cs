using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Portfolio.Queries;

public class GetPublicProjectsQuery : IRequest<List<object>>
{
}

public class GetPublicProjectsQueryHandler : IRequestHandler<GetPublicProjectsQuery, List<object>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetPublicProjectsQueryHandler> _logger;

    public GetPublicProjectsQueryHandler(IApplicationDbContext context, ILogger<GetPublicProjectsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<object>> Handle(GetPublicProjectsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching public projects");
            
            var projects = await _context.Projects
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.Id)
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
                    p.LiveUrl,
                    p.GithubUrl,
                    p.Color,
                    p.IsFeatured,
                    p.ViewsCount
                })
                .ToListAsync(cancellationToken);
            
            _logger.LogInformation("Retrieved {Count} public projects", projects.Count);
            return projects.Cast<object>().ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing {QueryName}", nameof(GetPublicProjectsQuery));
            throw;
        }
    }
}
