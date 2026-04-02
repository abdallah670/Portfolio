using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Messaging.Queries;

public class GetUnreadMessageCountQuery : IRequest<int>
{
}

public class GetUnreadMessageCountQueryHandler : IRequestHandler<GetUnreadMessageCountQuery, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetUnreadMessageCountQueryHandler> _logger;

    public GetUnreadMessageCountQueryHandler(IApplicationDbContext context, ILogger<GetUnreadMessageCountQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> Handle(GetUnreadMessageCountQuery request, CancellationToken cancellationToken)
    {
        var count = await _context.Messages.CountAsync(m => !m.IsRead, cancellationToken);
        _logger.LogDebug("Unread message count: {Count}", count);
        return count;
    }
}
