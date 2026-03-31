using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public record PublishProjectCommand(int ProjectId) : IRequest<bool>;

public class PublishProjectCommandHandler : IRequestHandler<PublishProjectCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public PublishProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(PublishProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.FindAsync(new object[] { request.ProjectId }, cancellationToken);

        if (project == null)
            return false;

        project.IsPublished = true;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public record UnpublishProjectCommand(int ProjectId) : IRequest<bool>;

public class UnpublishProjectCommandHandler : IRequestHandler<UnpublishProjectCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UnpublishProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UnpublishProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _context.Projects.FindAsync(new object[] { request.ProjectId }, cancellationToken);

        if (project == null)
            return false;

        project.IsPublished = false;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
