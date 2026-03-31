using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Settings.Queries;

public record GetSystemSettingsQuery(string? Category = null) : IRequest<List<SystemSetting>>;

public class GetSystemSettingsQueryHandler : IRequestHandler<GetSystemSettingsQuery, List<SystemSetting>>
{
    private readonly IApplicationDbContext _context;

    public GetSystemSettingsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SystemSetting>> Handle(GetSystemSettingsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.SystemSettings.AsQueryable();

        if (!string.IsNullOrEmpty(request.Category))
        {
            query = query.Where(s => s.Category == request.Category);
        }

        return await query.OrderBy(s => s.Category).ThenBy(s => s.Key).ToListAsync(cancellationToken);
    }
}

public record GetSystemSettingByKeyQuery(string Key) : IRequest<SystemSetting?>;

public class GetSystemSettingByKeyQueryHandler : IRequestHandler<GetSystemSettingByKeyQuery, SystemSetting?>
{
    private readonly IApplicationDbContext _context;

    public GetSystemSettingByKeyQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SystemSetting?> Handle(GetSystemSettingByKeyQuery request, CancellationToken cancellationToken)
    {
        return await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == request.Key, cancellationToken);
    }
}
