using FluentValidation;
using PortfolioApi.Application.DTOs;

namespace PortfolioApi.Api.Validators;

public class ChangeUsernameRequestValidator : AbstractValidator<ChangeUsernameRequest>
{
    public ChangeUsernameRequestValidator()
    {
        RuleFor(x => x.NewUsername)
            .NotEmpty().WithMessage("New username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters")
            .MaximumLength(50).WithMessage("Username must be at most 50 characters")
            .Matches(@"^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers, and underscores");
    }
}
