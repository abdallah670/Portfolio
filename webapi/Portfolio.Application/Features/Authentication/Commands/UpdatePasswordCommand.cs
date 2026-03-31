using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Authentication.Commands;

public record UpdatePasswordCommand(string CurrentPassword, string NewPassword, string Username)
    : IRequest<PasswordUpdateResult>;

public class PasswordUpdateResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, PasswordUpdateResult>
{
    private readonly UserManager<AdminUser> _userManager;

    public UpdatePasswordCommandHandler(UserManager<AdminUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<PasswordUpdateResult> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Username))
        {
            return new PasswordUpdateResult
            {
                Success = false,
                Errors = new List<string> { "User not authenticated" }
            };
        }

        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null)
        {
            return new PasswordUpdateResult
            {
                Success = false,
                Errors = new List<string> { "User not found" }
            };
        }

        // Verify current password
        var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);

        if (!isCurrentPasswordValid)
        {
            return new PasswordUpdateResult
            {
                Success = false,
                Errors = new List<string> { "Current password is incorrect" }
            };
        }

        // Change password
        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
        {
            return new PasswordUpdateResult { Success = true };
        }

        return new PasswordUpdateResult
        {
            Success = false,
            Errors = result.Errors.Select(e => e.Description).ToList()
        };
    }
}
