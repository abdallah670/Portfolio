using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Messaging.Commands;

public class MarkMessageAsReadCommand : IRequest<bool>
{
    public int Id { get; set; }
}

public class MarkAllMessagesAsReadCommand : IRequest<int>
{
    public int Count { get; set; }
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

public class MarkAllMessagesAsReadCommandHandler : IRequestHandler<MarkAllMessagesAsReadCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<MarkAllMessagesAsReadCommandHandler> _logger;

    public MarkAllMessagesAsReadCommandHandler(IApplicationDbContext context, ILogger<MarkAllMessagesAsReadCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> Handle(MarkAllMessagesAsReadCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Marking all unread messages as read");

        var unreadMessages = await _context.Messages
            .Where(m => !m.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var message in unreadMessages)
        {
            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var count = unreadMessages.Count;
        _logger.LogInformation("{Count} messages marked as read", count);
        return count;
    }
}

