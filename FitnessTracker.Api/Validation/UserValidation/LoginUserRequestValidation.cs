using FitnessTrackerApi.Contracts.UserContracts;
using FluentValidation;

namespace FitnessTrackerApi.Validation.UserValidation;

public class LoginUserRequestValidation : AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValidation()
    {
        RuleFor(u=> u.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address")
            .MaximumLength(100).WithMessage("Email must be less than 100 characters");
        RuleFor(u=> u.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters");
    }
}