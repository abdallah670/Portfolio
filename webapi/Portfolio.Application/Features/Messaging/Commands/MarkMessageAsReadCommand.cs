using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Messaging.Commands;

public class MarkMessageAsReadCommand : IRequest<bool>
{
    public int Id { get; set; }
}

public class MarkMessageAsReadCommandHandler : IRequestHandler<MarkMessageAsReadCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<MarkMessageAsReadCommandHandler> _logger;

    public MarkMessageAsReadCommandHandler(IApplicationDbContext context, ILogger<MarkMessageAsReadCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(MarkMessageAsReadCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Marking message {MessageId} as read", request.Id);
        
        var message = await _context.Messages.FindAsync(new object[] { request.Id }, cancellationToken);
        if (message == null)
        {
            _logger.LogWarning("Message {MessageId} not found", request.Id);
            return false;
        }

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Message {MessageId} marked as read", request.Id);
        return true;
    }
}
