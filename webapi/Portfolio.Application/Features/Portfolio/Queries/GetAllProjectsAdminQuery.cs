using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Application.DTOs;

namespace PortfolioApi.Application.Features.Portfolio.Queries;

public class GetAllProjectsAdminQuery : IRequest<PaginatedResponse<object>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetAllProjectsAdminQueryHandler : IRequestHandler<GetAllProjectsAdminQuery, PaginatedResponse<object>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetAllProjectsAdminQueryHandler> _logger;

    public GetAllProjectsAdminQueryHandler(IApplicationDbContext context, ILogger<GetAllProjectsAdminQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PaginatedResponse<object>> Handle(GetAllProjectsAdminQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Admin fetching paginated projects page {Page} with size {PageSize}", request.Page, request.PageSize);
        
        var totalCount = await _context.Projects.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var projects = await _context.Projects
            .OrderByDescending(p => p.IsFeatured)
            .ThenBy(p => p.DisplayOrder)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
                p.LiveUrl,
                p.Color,
                p.IsFeatured,
                p.IsPublished,
                p.DisplayOrder,
                p.ViewsCount
            })
            .ToListAsync(cancellationToken);
        
        _logger.LogInformation("Admin retrieved {Count} projects of total {TotalCount}", projects.Count, totalCount);
        
        return new PaginatedResponse<object>
        {
            Items = projects.Cast<object>().ToList(),
            TotalCount = totalCount,
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };
    }
}
