using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Projects.Commands;

public record IncrementProjectViewsCommand(int ProjectId) : IRequest<bool>;

public class IncrementProjectViewsCommandHandler : IRequestHandler<IncrementProjectViewsCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<IncrementProjectViewsCommandHandler> _logger;

    public IncrementProjectViewsCommandHandler(IApplicationDbContext context, ILogger<IncrementProjectViewsCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(IncrementProjectViewsCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken);
        
        if (project == null)
        {
            _logger.LogWarning("Project {ProjectId} not found for view increment", request.ProjectId);
            return false;
        }

        project.ViewsCount++;
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Incremented views for project {ProjectId} to {ViewsCount}", 
            request.ProjectId, project.ViewsCount);
        
        return true;
    }
}
