using FitnessTrackerApi.Contracts.UserContracts;
using FluentValidation;

namespace FitnessTrackerApi.Validation.UserValidation;

public class RegisterUserRequestValidation : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidation()
    {
        RuleFor(u=> u.Username)
            .NotEmpty().WithMessage("Username is required")
            .MaximumLength(100).WithMessage("Username must be less than 100 characters");
        RuleFor(u=> u.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address")
            .MaximumLength(100).WithMessage("Email must be less than 100 characters");
        RuleFor(u=> u.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
}