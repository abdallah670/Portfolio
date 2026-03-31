using MediatR;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Messaging.Commands;

public record CreateMessageCommand(
    string Name,
    string Email,
    string? Subject,
    string Content,
    string? IpAddress,
    string? UserAgent
) : IRequest<ApiResponse<int>>;

public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, ApiResponse<int>>
{
    private readonly IApplicationDbContext _context;

    public CreateMessageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<int>> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new Message
        {
            Name = request.Name,
            Email = request.Email,
            Subject = request.Subject ?? string.Empty,
            Content = request.Content,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);

        return new ApiResponse<int> 
        { 
            Success = true, 
            Message = "Message sent successfully", 
            Data = message.Id 
        };
    }
}
