using FluentValidation;
using PortfolioApi.Api.Models;

namespace PortfolioApi.Api.Validators;

public class RespondToMessageRequestValidator : AbstractValidator<RespondToMessageRequest>
{
    public RespondToMessageRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Response content is required")
            .MinimumLength(10).WithMessage("Response must be at least 10 characters")
            .MaximumLength(5000).WithMessage("Response must be at most 5000 characters");
    }
}
