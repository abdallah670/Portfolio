using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class DeleteProjectCommand : IRequest<bool>
{
    public int Id { get; set; }
}

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteProjectCommandHandler> _logger;

    public DeleteProjectCommandHandler(IApplicationDbContext context, ILogger<DeleteProjectCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to delete project {ProjectId}", request.Id);
        
        var project = await _context.Projects.FindAsync(new object[] { request.Id }, cancellationToken);
        if (project == null)
        {
            _logger.LogWarning("Project {ProjectId} not found for deletion", request.Id);
            return false;
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Project {ProjectId} deleted successfully", request.Id);
        return true;
    }
}
