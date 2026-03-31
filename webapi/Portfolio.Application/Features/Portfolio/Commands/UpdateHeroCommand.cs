using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Domain.Entities;
using PortfolioApi.Infrastructure.Data;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public record UpdateHeroCommand(Hero Hero) : IRequest<ApiResponse<Hero>>;

public class UpdateHeroCommandHandler : IRequestHandler<UpdateHeroCommand, ApiResponse<Hero>>
{
    private readonly AppDbContext _context;

    public UpdateHeroCommandHandler(AppDbContext context)
    {
        _context = context;
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
        hero.AvailabilityLabel = request.Hero.AvailabilityLabel;
        hero.Subtitle = request.Hero.Subtitle;
        hero.HeroIntro = request.Hero.HeroIntro;
        hero.CtaPrimaryLabel = request.Hero.CtaPrimaryLabel;
        hero.CtaPrimaryHref = request.Hero.CtaPrimaryHref;
        hero.CtaSecondaryLabel = request.Hero.CtaSecondaryLabel;
        hero.CtaSecondaryHref = request.Hero.CtaSecondaryHref;
        hero.ProfileImage = request.Hero.ProfileImage;
        hero.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(cancellationToken);
        return new ApiResponse<Hero> { Success = true, Message = "Hero updated", Data = hero };
    }
}
