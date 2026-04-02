using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class CreateJourneyCommand : IRequest<JourneyItem>
{
    public JourneyItem Item { get; set; } = null!;
}

public class CreateJourneyCommandHandler : IRequestHandler<CreateJourneyCommand, JourneyItem>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateJourneyCommandHandler> _logger;

    public CreateJourneyCommandHandler(IApplicationDbContext context, ILogger<CreateJourneyCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<JourneyItem> Handle(CreateJourneyCommand request, CancellationToken cancellationToken)
    {
        
        _logger.LogInformation("Creating new journey item: {Title}", request.Item.Title);
        
        _context.JourneyItems.Add(request.Item);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Journey item {JourneyId} created successfully", request.Item.Id);
        return request.Item;
    }
}
