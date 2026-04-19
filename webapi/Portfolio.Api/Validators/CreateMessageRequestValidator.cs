using FluentValidation;
using PortfolioApi.Api.Models;

namespace PortfolioApi.Api.Validators;

public class CreateMessageRequestValidator : AbstractValidator<CreateMessageRequest>
{
    public CreateMessageRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters")
            .MaximumLength(100).WithMessage("Name must be at most 100 characters")
            .Matches(@"^[a-zA-Z\s'-]+$").WithMessage("Name contains invalid characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(200).WithMessage("Email must be at most 200 characters");

        RuleFor(x => x.Subject)
            .MaximumLength(200).WithMessage("Subject must be at most 200 characters")
            .When(x => !string.IsNullOrEmpty(x.Subject));

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Message content is required")
            .MinimumLength(10).WithMessage("Message must be at least 10 characters")
            .MaximumLength(5000).WithMessage("Message must be at most 5000 characters");
    }
}
