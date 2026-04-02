using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Messaging.Queries;

public class GetMessageByIdQuery : IRequest<Message?>
{
    public int Id { get; set; }
}

public class GetMessageByIdQueryHandler : IRequestHandler<GetMessageByIdQuery, Message?>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetMessageByIdQueryHandler> _logger;

    public GetMessageByIdQueryHandler(IApplicationDbContext context, ILogger<GetMessageByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Message?> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching message {MessageId}", request.Id);
        
        var message = await _context.Messages.FindAsync(new object[] { request.Id }, cancellationToken);
        
        if (message == null)
        {
            _logger.LogWarning("Message {MessageId} not found", request.Id);
            return null;
        }
        
        _logger.LogInformation("Message {MessageId} retrieved successfully", request.Id);
        return message;
    }
}
