using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Settings.Commands;

public record UpdateSystemSettingCommand(string Key, string Value, string DataType, string? UpdatedBy = null)
    : IRequest<bool>;

public class UpdateSystemSettingCommandHandler : IRequestHandler<UpdateSystemSettingCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateSystemSettingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateSystemSettingCommand request, CancellationToken cancellationToken)
    {
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == request.Key, cancellationToken);

        if (setting == null)
        {
            // Create new setting
            setting = new SystemSetting
            {
                Key = request.Key,
                Value = request.Value,
                DataType = request.DataType,
                UpdatedBy = request.UpdatedBy,
                UpdatedAt = DateTime.UtcNow
            };
            _context.SystemSettings.Add(setting);
        }
        else
        {
            // Update existing
            setting.Value = request.Value;
            setting.DataType = request.DataType;
            setting.UpdatedBy = request.UpdatedBy;
            setting.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
