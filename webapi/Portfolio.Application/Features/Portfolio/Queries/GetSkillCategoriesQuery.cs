using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Portfolio.Queries;

public record GetSkillCategoriesQuery() : IRequest<List<SkillCategoryDto>>;

public class GetSkillCategoriesQueryHandler : IRequestHandler<GetSkillCategoriesQuery, List<SkillCategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSkillCategoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SkillCategoryDto>> Handle(GetSkillCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.SkillCategories
            .Include(c => c.Skills)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(cancellationToken);
        
        return categories.Select(c => new SkillCategoryDto
        {
            Title = c.Title,
            Color = c.Color,
            Skills = c.Skills.Select(s => new SkillDto { Name = s.Name, Level = s.Level }).ToList()
        }).ToList();
    }
}
