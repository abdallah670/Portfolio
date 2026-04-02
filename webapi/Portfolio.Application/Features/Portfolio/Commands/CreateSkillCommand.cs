using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class CreateSkillCommand : IRequest<Skill>
{
    public Skill Skill { get; set; } = null!;
}

public class CreateSkillCommandHandler : IRequestHandler<CreateSkillCommand, Skill>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateSkillCommandHandler> _logger;

    public CreateSkillCommandHandler(IApplicationDbContext context, ILogger<CreateSkillCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Skill> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new skill: {Name} in category {CategoryId}", 
            request.Skill.Name, request.Skill.CategoryId);
        
        _context.Skills.Add(request.Skill);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Skill {SkillId} created successfully", request.Skill.Id);
        return request.Skill;
    }
}
