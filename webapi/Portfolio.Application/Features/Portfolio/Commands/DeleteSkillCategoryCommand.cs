using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class DeleteSkillCategoryCommand : IRequest<bool>
{
    public int Id { get; set; }
}

public class DeleteSkillCategoryCommandHandler : IRequestHandler<DeleteSkillCategoryCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteSkillCategoryCommandHandler> _logger;

    public DeleteSkillCategoryCommandHandler(IApplicationDbContext context, ILogger<DeleteSkillCategoryCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteSkillCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to delete skill category {CategoryId}", request.Id);
        
        var category = await _context.SkillCategories.FindAsync(new object[] { request.Id }, cancellationToken);
        if (category == null)
        {
            _logger.LogWarning("Skill category {CategoryId} not found for deletion", request.Id);
            return false;
        }

        _context.SkillCategories.Remove(category);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Skill category {CategoryId} deleted successfully", request.Id);
        return true;
    }
}