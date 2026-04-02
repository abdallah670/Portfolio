using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class CreateSkillCategoryCommand : IRequest<SkillCategory>
{
    public SkillCategory Category { get; set; } = null!;
}

public class CreateSkillCategoryCommandHandler : IRequestHandler<CreateSkillCategoryCommand, SkillCategory>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateSkillCategoryCommandHandler> _logger;

    public CreateSkillCategoryCommandHandler(IApplicationDbContext context, ILogger<CreateSkillCategoryCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SkillCategory> Handle(CreateSkillCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new skill category: {Title}", request.Category.Title);
        
        _context.SkillCategories.Add(request.Category);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Skill category {CategoryId} created successfully", request.Category.Id);
        return request.Category;
    }
}
