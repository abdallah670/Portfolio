using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class UpdateJourneyCommand : IRequest<JourneyItem>
{
    public JourneyItem Item { get; set; } = null!;
}

public class UpdateJourneyCommandHandler : IRequestHandler<UpdateJourneyCommand, JourneyItem>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateJourneyCommandHandler> _logger;

    public UpdateJourneyCommandHandler(IApplicationDbContext context, ILogger<UpdateJourneyCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<JourneyItem> Handle(UpdateJourneyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating journey item {JourneyId}", request.Item.Id);
        
        _context.JourneyItems.Update(request.Item);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Journey item {JourneyId} updated successfully", request.Item.Id);
        return request.Item;
    }
}
