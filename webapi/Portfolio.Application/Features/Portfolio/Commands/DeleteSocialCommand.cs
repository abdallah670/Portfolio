using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class DeleteSocialCommand : IRequest<bool>
{
    public int Id { get; set; }
}

public class DeleteSocialCommandHandler : IRequestHandler<DeleteSocialCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteSocialCommandHandler> _logger;

    public DeleteSocialCommandHandler(IApplicationDbContext context, ILogger<DeleteSocialCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteSocialCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to delete social link {SocialId}", request.Id);
        
        var social = await _context.SocialLinks.FindAsync(new object[] { request.Id }, cancellationToken);
        if (social == null)
        {
            _logger.LogWarning("Social link {SocialId} not found for deletion", request.Id);
            return false;
        }

        _context.SocialLinks.Remove(social);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Social link {SocialId} deleted successfully", request.Id);
        return true;
    }
}
