using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class DeleteSkillCommand : IRequest<bool>
{
    public int Id { get; set; }
}

public class DeleteSkillCommandHandler : IRequestHandler<DeleteSkillCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteSkillCommandHandler> _logger;

    public DeleteSkillCommandHandler(IApplicationDbContext context, ILogger<DeleteSkillCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to delete skill {SkillId}", request.Id);
        
        var skill = await _context.Skills.FindAsync(new object[] { request.Id }, cancellationToken);
        if (skill == null)
        {
            _logger.LogWarning("Skill {SkillId} not found for deletion", request.Id);
            return false;
        }

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Skill {SkillId} deleted successfully", request.Id);
        return true;
    }
}
