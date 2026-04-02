using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class DeleteJourneyCommand : IRequest<bool>
{
    public int Id { get; set; }
}

public class DeleteJourneyCommandHandler : IRequestHandler<DeleteJourneyCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteJourneyCommandHandler> _logger;

    public DeleteJourneyCommandHandler(IApplicationDbContext context, ILogger<DeleteJourneyCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteJourneyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to delete journey item {JourneyId}", request.Id);
        
        var item = await _context.JourneyItems.FindAsync(new object[] { request.Id }, cancellationToken);
        if (item == null)
        {
            _logger.LogWarning("Journey item {JourneyId} not found for deletion", request.Id);
            return false;
        }

        _context.JourneyItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Journey item {JourneyId} deleted successfully", request.Id);
        return true;
    }
}
