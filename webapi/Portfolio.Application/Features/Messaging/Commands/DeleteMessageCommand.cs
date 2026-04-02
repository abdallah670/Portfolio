using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Messaging.Commands;

public class DeleteMessageCommand : IRequest<bool>
{
    public int Id { get; set; }
}

public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteMessageCommandHandler> _logger;

    public DeleteMessageCommandHandler(IApplicationDbContext context, ILogger<DeleteMessageCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to delete message {MessageId}", request.Id);
        
        var message = await _context.Messages.FindAsync(new object[] { request.Id }, cancellationToken);
        if (message == null)
        {
            _logger.LogWarning("Message {MessageId} not found for deletion", request.Id);
            return false;
        }

        _context.Messages.Remove(message);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Message {MessageId} deleted successfully", request.Id);
        return true;
    }
}
