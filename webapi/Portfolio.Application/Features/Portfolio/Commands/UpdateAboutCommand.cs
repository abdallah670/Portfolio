using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public record UpdateAboutCommand(About About) : IRequest<ApiResponse<About>>;

public class UpdateAboutCommandHandler : IRequestHandler<UpdateAboutCommand, ApiResponse<About>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateAboutCommandHandler> _logger;

    public UpdateAboutCommandHandler(IApplicationDbContext context, ILogger<UpdateAboutCommandHandler> logger)
    {
        _context = context;
        _logger = logger;

    }

    public async Task<ApiResponse<About>> Handle(UpdateAboutCommand request, CancellationToken cancellationToken)
    {
        var about = await _context.Abouts.FirstOrDefaultAsync(cancellationToken);
        if (about == null)
        {
            about = new About { Id = 0 };
            _context.Abouts.Add(about);
        }
        
        about.Kicker = request.About.Kicker;
        about.Title = request.About.Title;
        about.Subtitle = request.About.Subtitle;
        about.FunFact = request.About.FunFact;
        about.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("About section updated successfully at {UpdatedAt}", about.UpdatedAt);
        return new ApiResponse<About> { Success = true, Message = "About updated", Data = about };
    }
}
