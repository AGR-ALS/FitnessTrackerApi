using FitnessTrackerApi.Contracts.WorkoutsContracts;
using FluentValidation;

namespace FitnessTrackerApi.Validation.WorkoutsValidation;

public class PostPutSetRequestValidator : AbstractValidator<PostPutSetRequest>
{
    public PostPutSetRequestValidator()
    {
        RuleFor(s => s.Reps)
            .NotEmpty().WithMessage("Reps are required")
            .GreaterThanOrEqualTo(1).WithMessage("Reps must be at least 1");
        RuleFor(s=>s.Weight)
            .NotEmpty().WithMessage("Weight is required")
            .GreaterThan(0).WithMessage("Weight cannot be less than zero");
    }
}