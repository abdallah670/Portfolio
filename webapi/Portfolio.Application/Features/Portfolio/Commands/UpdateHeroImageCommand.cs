using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class UpdateHeroImageCommand : IRequest<ApiResponse<string>>
{
    public string ImagePath { get; set; }
}
public class UpdateHeroImageCommandHandler : IRequestHandler<UpdateHeroImageCommand, ApiResponse<string>>
{
    private readonly IApplicationDbContext _context;

    public UpdateHeroImageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<string>> Handle(UpdateHeroImageCommand request, CancellationToken cancellationToken)
    {
        var hero = await _context.Heroes.FirstOrDefaultAsync(cancellationToken);
        if (hero == null)
        {
            hero = new Hero { Id = 0 };
            _context.Heroes.Add(hero);
        }
        
        hero.ProfileImage = request.ImagePath;
        hero.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync(cancellationToken);
        return new ApiResponse<string> { Success = true, Message = "Profile image updated", Data = hero.ProfileImage };
    }
}
