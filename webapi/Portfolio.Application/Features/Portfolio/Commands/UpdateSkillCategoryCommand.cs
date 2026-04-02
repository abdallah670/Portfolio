using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class UpdateSkillCategoryCommand : IRequest<SkillCategory>
{
    public SkillCategory Category { get; set; } = null!;
}

public class UpdateSkillCategoryCommandHandler : IRequestHandler<UpdateSkillCategoryCommand, SkillCategory>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateSkillCategoryCommandHandler> _logger;

    public UpdateSkillCategoryCommandHandler(IApplicationDbContext context, ILogger<UpdateSkillCategoryCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SkillCategory> Handle(UpdateSkillCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating skill category {CategoryId}", request.Category.Id);
        
        _context.SkillCategories.Update(request.Category);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Skill category {CategoryId} updated successfully", request.Category.Id);
        return request.Category;
    }
}
