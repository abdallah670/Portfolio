using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class UpdateProjectCommand : IRequest<Project>
{
    public Project Project { get; set; } = null!;
}

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Project>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateProjectCommandHandler> _logger;

    public UpdateProjectCommandHandler(IApplicationDbContext context, ILogger<UpdateProjectCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Project> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating project {ProjectId}", request.Project.Id);
        
        _context.Projects.Update(request.Project);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Project {ProjectId} updated successfully", request.Project.Id);
        return request.Project;
    }
}
