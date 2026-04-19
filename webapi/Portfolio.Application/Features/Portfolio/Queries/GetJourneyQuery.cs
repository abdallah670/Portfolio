using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Portfolio.Queries;

public record GetJourneyQuery() : IRequest<List<JourneyItemDto>>;

public class GetJourneyQueryHandler : IRequestHandler<GetJourneyQuery, List<JourneyItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetJourneyQueryHandler> _logger;

    public GetJourneyQueryHandler(IApplicationDbContext context, ILogger<GetJourneyQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<JourneyItemDto>> Handle(GetJourneyQuery request, CancellationToken cancellationToken)
    {
        var journey = await _context.JourneyItems
            .OrderBy(j => j.DisplayOrder)
            .ThenBy(j => j.Id)
            .ToListAsync(cancellationToken);
        
        _logger.LogInformation("Retrieved {Count} journey items", journey.Count);
        
        return journey.Select(j => new JourneyItemDto
        {
            Id = j.Id,
            Title = j.Title,
            Period = j.Period,
            Org = j.Org,
            Description = j.Description,
            DisplayOrder = j.DisplayOrder
        }).ToList();
    }
}
