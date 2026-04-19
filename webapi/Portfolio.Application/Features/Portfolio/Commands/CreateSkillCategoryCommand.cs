using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class CreateSkillCategoryCommand : IRequest<SkillCategoryDto>
{
    public SkillCategory Category { get; set; } = null!;
}

public class CreateSkillCategoryCommandHandler : IRequestHandler<CreateSkillCategoryCommand, SkillCategoryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateSkillCategoryCommandHandler> _logger;

    public CreateSkillCategoryCommandHandler(IApplicationDbContext context, ILogger<CreateSkillCategoryCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SkillCategoryDto> Handle(CreateSkillCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new skill category: {Title}", request.Category.Title);
        
        _context.SkillCategories.Add(request.Category);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Skill category {CategoryId} created successfully", request.Category.Id);
        
        // Return DTO to avoid circular reference
        return new SkillCategoryDto
        {
            Id = request.Category.Id,
            Title = request.Category.Title,
            Color = request.Category.Color,
            Skills = request.Category.Skills?.Select(s => new SkillDto
            {
                Id = s.Id,
                Name = s.Name,
                Level = s.Level
            }).ToList() ?? new List<SkillDto>()
        };
    }
}
