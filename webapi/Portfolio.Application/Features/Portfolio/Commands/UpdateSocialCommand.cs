using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class UpdateSocialCommand : IRequest<SocialLink>
{
    public SocialLink Social { get; set; } = null!;
}

public class UpdateSocialCommandHandler : IRequestHandler<UpdateSocialCommand, SocialLink>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateSocialCommandHandler> _logger;

    public UpdateSocialCommandHandler(IApplicationDbContext context, ILogger<UpdateSocialCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SocialLink> Handle(UpdateSocialCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating social link {SocialId}", request.Social.Id);
        
        _context.SocialLinks.Update(request.Social);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Social link {SocialId} updated successfully", request.Social.Id);
        return request.Social;
    }
}
