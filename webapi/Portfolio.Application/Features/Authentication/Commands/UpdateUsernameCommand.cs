using MediatR;
using Microsoft.AspNetCore.Identity;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Authentication.Commands;

public record UpdateUsernameCommand(string NewUsername, string Username)
    : IRequest<UsernameUpdateResult>;

public class UsernameUpdateResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class UpdateUsernameCommandHandler : IRequestHandler<UpdateUsernameCommand, UsernameUpdateResult>
{
    private readonly UserManager<AdminUser> _userManager;

    public UpdateUsernameCommandHandler(UserManager<AdminUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UsernameUpdateResult> Handle(UpdateUsernameCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Username))
        {
            return new UsernameUpdateResult
            {
                Success = false,
                Errors = new List<string> { "User not authenticated" }
            };
        }

        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null)
        {
            return new UsernameUpdateResult
            {
                Success = false,
                Errors = new List<string> { "User not found" }
            };
        }

        if (string.IsNullOrEmpty(request.NewUsername))
        {
            return new UsernameUpdateResult
            {
                Success = false,
                Errors = new List<string> { "New username is required" }
            };
        }

        var existingUser = await _userManager.FindByNameAsync(request.NewUsername);
        if (existingUser != null && existingUser.Id != user.Id)
        {
            return new UsernameUpdateResult
            {
                Success = false,
                Errors = new List<string> { "Username is already taken" }
            };
        }

        var result = await _userManager.SetUserNameAsync(user, request.NewUsername);

        if (result.Succeeded)
        {
            return new UsernameUpdateResult { Success = true };
        }

        return new UsernameUpdateResult
        {
            Success = false,
            Errors = result.Errors.Select(e => e.Description).ToList()
        };
    }
}