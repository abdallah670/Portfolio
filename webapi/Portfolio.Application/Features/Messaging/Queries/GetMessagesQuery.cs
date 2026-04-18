using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Messaging.Queries;

public class GetMessagesQuery : IRequest<PagedMessagesResult>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool? IsRead { get; set; }
}

public class PagedMessagesResult
{
    public List<MessageDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class MessageDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Preview { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public bool IsReplied { get; set; }
    public DateTime? RepliedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, PagedMessagesResult>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetMessagesQueryHandler> _logger;

    public GetMessagesQueryHandler(IApplicationDbContext context, ILogger<GetMessagesQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedMessagesResult> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching messages page {Page} with page size {PageSize}", request.Page, request.PageSize);
        
        var query = _context.Messages.AsQueryable();

        if (request.IsRead.HasValue)
        {
            query = query.Where(m => m.IsRead == request.IsRead.Value);
            _logger.LogDebug("Filtering by IsRead={IsRead}", request.IsRead.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var messages = await query
            .OrderByDescending(m => m.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Subject = m.Subject,
                Preview = m.Content.Length > 100 ? m.Content.Substring(0, 100) + "..." : m.Content,
                IsRead = m.IsRead,
                IsReplied = m.IsReplied,
                RepliedAt = m.RepliedAt,
                CreatedAt = m.CreatedAt
            })
            .ToListAsync(cancellationToken);

        _logger.LogInformation("Retrieved {Count} messages out of {Total} total", messages.Count, totalCount);
        
        return new PagedMessagesResult
        {
            Items = messages,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}
