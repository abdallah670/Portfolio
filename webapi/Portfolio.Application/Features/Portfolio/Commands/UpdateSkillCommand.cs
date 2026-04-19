using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class UpdateSkillCommand : IRequest<Skill>
{
    public Skill Skill { get; set; } = null!;
}

public class UpdateSkillCommandHandler : IRequestHandler<UpdateSkillCommand, Skill>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateSkillCommandHandler> _logger;

    public UpdateSkillCommandHandler(IApplicationDbContext context, ILogger<UpdateSkillCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Skill> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating skill {SkillId}: {Name}", request.Skill.Id, request.Skill.Name);
        
        var skill = await _context.Skills.FindAsync(new object[] { request.Skill.Id }, cancellationToken);
        if (skill == null)
        {
            _logger.LogWarning("Skill {SkillId} not found", request.Skill.Id);
            throw new Exception("Skill not found");
        }

        skill.Name = request.Skill.Name;
        skill.Level = request.Skill.Level;
        
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Skill {SkillId} updated successfully", skill.Id);
        return skill;
    }
}