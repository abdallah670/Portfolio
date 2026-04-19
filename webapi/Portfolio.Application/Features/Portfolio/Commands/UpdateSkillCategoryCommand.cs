using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class UpdateSkillCategoryCommand : IRequest<SkillCategoryDto>
{
    public SkillCategory Category { get; set; } = null!;
}

public class UpdateSkillCategoryCommandHandler : IRequestHandler<UpdateSkillCategoryCommand, SkillCategoryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateSkillCategoryCommandHandler> _logger;

    public UpdateSkillCategoryCommandHandler(IApplicationDbContext context, ILogger<UpdateSkillCategoryCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SkillCategoryDto> Handle(UpdateSkillCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating skill category {CategoryId}", request.Category.Id);
        
        var existingCategory = await _context.SkillCategories
            .Include(c => c.Skills)
            .FirstOrDefaultAsync(c => c.Id == request.Category.Id, cancellationToken);
        
        if (existingCategory == null)
        {
            throw new Exception($"Skill category {request.Category.Id} not found");
        }
        
        // Update category properties
        existingCategory.Title = request.Category.Title;
        existingCategory.Color = request.Category.Color;
        existingCategory.DisplayOrder = request.Category.DisplayOrder;
        
        // Handle skills - remove old ones and add new/updated
        var skillsToRemove = existingCategory.Skills.ToList();
        foreach (var skill in skillsToRemove)
        {
            _context.Skills.Remove(skill);
        }
        
        // Add all skills from request as new entities (IDs reset to 0)
        foreach (var skill in request.Category.Skills)
        {
            skill.Id = 0; // Reset ID to treat as new entity
            skill.CategoryId = existingCategory.Id;
            skill.Category = null!; // Navigation property will be set by EF
            _context.Skills.Add(skill);
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Skill category {CategoryId} updated successfully with {SkillCount} skills", 
            request.Category.Id, request.Category.Skills.Count);
        
        // Return DTO to avoid circular reference
        return new SkillCategoryDto
        {
            Id = existingCategory.Id,
            Title = existingCategory.Title,
            Color = existingCategory.Color,
            Skills = existingCategory.Skills.Select(s => new SkillDto
            {
                Id = s.Id,
                Name = s.Name,
                Level = s.Level
            }).ToList()
        };
    }
}
