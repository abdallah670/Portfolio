using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Messaging.Commands;

public class RespondToMessageCommand : IRequest<RespondToMessageResult>
{
    public int MessageId { get; set; }
    public string ResponseContent { get; set; } = string.Empty;
}

public class RespondToMessageResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}

public class RespondToMessageCommandHandler : IRequestHandler<RespondToMessageCommand, RespondToMessageResult>
{
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<RespondToMessageCommandHandler> _logger;

    public RespondToMessageCommandHandler(
        IApplicationDbContext context, 
        IEmailService emailService,
        ILogger<RespondToMessageCommandHandler> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<RespondToMessageResult> Handle(RespondToMessageCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing response to message {MessageId}", request.MessageId);
        
        var message = await _context.Messages.FindAsync(new object[] { request.MessageId }, cancellationToken);
        if (message == null)
        {
            _logger.LogWarning("Message {MessageId} not found", request.MessageId);
            return new RespondToMessageResult { Success = false, Error = "Message not found" };
        }

        // Build email body with proper greeting and signature
        var emailBody = $@"<p>Hi {message.Name},</p>
<p>Thank you for reaching out. Regarding your message:</p>
<blockquote style='border-left: 3px solid #ccc; margin-left: 0; padding-left: 1em; color: #666;'>
    <p><strong>Your message:</strong></p>
    <p>{message.Content}</p>
</blockquote>
<p><strong>My response:</strong></p>
<p>{request.ResponseContent}</p>
<br>
<p>Best regards,<br>Abdullah Mohamed</p>";

        try
        {
            _logger.LogInformation("Sending email response to {Email}", message.Email);
            
            await _emailService.SendEmailAsync(
                message.Email,
                $"Re: {message.Subject}",
                emailBody
            );
            
            _logger.LogInformation("Email sent successfully to {Email}", message.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email response to {Email}", message.Email);
            return new RespondToMessageResult { Success = false, Error = $"Failed to send email: {ex.Message}" };
        }

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;
        message.IsReplied = true;
        message.RepliedAt = DateTime.UtcNow;
        message.ReplyContent = request.ResponseContent;

        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Message {MessageId} marked as replied successfully", request.MessageId);
        return new RespondToMessageResult { Success = true };
    }
}
