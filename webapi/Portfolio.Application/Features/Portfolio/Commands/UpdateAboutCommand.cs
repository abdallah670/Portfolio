using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Domain.Entities;
using PortfolioApi.Infrastructure.Data;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public record UpdateAboutCommand(About About) : IRequest<ApiResponse<About>>;

public class UpdateAboutCommandHandler : IRequestHandler<UpdateAboutCommand, ApiResponse<About>>
{
    private readonly AppDbContext _context;

    public UpdateAboutCommandHandler(AppDbContext context)
    {
        _context = context;
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
        return new ApiResponse<About> { Success = true, Message = "About updated", Data = about };
    }
}
