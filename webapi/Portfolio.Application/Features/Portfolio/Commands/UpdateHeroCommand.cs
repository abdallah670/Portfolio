using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public record UpdateHeroCommand(Hero Hero, List<HeroStats>? Stats) : IRequest<ApiResponse<Hero>>;

public class UpdateHeroCommandHandler : IRequestHandler<UpdateHeroCommand, ApiResponse<Hero>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateHeroCommandHandler> _logger;

    public UpdateHeroCommandHandler(IApplicationDbContext context, ILogger<UpdateHeroCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<Hero>> Handle(UpdateHeroCommand request, CancellationToken cancellationToken)
    {
        var hero = await _context.Heroes.FirstOrDefaultAsync(cancellationToken);
        if (hero == null)
        {
            hero = new Hero { Id = 0 };
            _context.Heroes.Add(hero);
        }
        
        hero.Name = request.Hero.Name;
        hero.HeadlineTop = request.Hero.HeadlineTop;
        hero.HeadlineMain = request.Hero.HeadlineMain;
        hero.Subtitle = request.Hero.Subtitle;
        hero.AvailabilityLabel = request.Hero.AvailabilityLabel;
        hero.ProfileImage = request.Hero.ProfileImage;
        hero.UpdatedAt = DateTime.UtcNow;
        
        // Handle Stats - remove existing and add new
        if (request.Stats != null)
        {
            var existingStats = await _context.HeroStats.ToListAsync(cancellationToken);
            _context.HeroStats.RemoveRange(existingStats);
            
            for (int i = 0; i < request.Stats.Count; i++)
            {
                var stat = request.Stats[i];
                _context.HeroStats.Add(new HeroStats
                {
                    Label = stat.Label,
                    Value = stat.Value,
                    DisplayOrder = i
                });
            }
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Hero section updated successfully at {UpdatedAt}", hero.UpdatedAt);
        return new ApiResponse<Hero> { Success = true, Message = "Hero updated", Data = hero };
    }
}
