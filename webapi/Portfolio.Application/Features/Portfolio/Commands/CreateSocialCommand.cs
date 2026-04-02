using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class CreateSocialCommand : IRequest<SocialLink>
{
    public SocialLink Social { get; set; } = null!;
}

public class CreateSocialCommandHandler : IRequestHandler<CreateSocialCommand, SocialLink>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateSocialCommandHandler> _logger;

    public CreateSocialCommandHandler(IApplicationDbContext context, ILogger<CreateSocialCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SocialLink> Handle(CreateSocialCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new social link: {Label}", request.Social.Label);
        
        _context.SocialLinks.Add(request.Social);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Social link {SocialId} created successfully", request.Social.Id);
        return request.Social;
    }
}
