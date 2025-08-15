using FitnessTrackerApi.Contracts.WorkoutsContracts;
using FluentValidation;

namespace FitnessTrackerApi.Validation.WorkoutsValidation;

public class PostPutExerciseRequestValidator : AbstractValidator<PostPutExerciseRequest>
{
    public PostPutExerciseRequestValidator()
    {
        RuleFor(ex => ex.Name)
            .NotEmpty().WithMessage("Exercise name is required")
            .MaximumLength(100).WithMessage("Exercise name must be less than 100 characters");
        RuleFor(ex => ex.Sets)
            .NotEmpty().WithMessage("Exercise sets are required")
            .Must(s => s.Count >= 1).WithMessage("At least one set must be provided");
        RuleForEach(ex => ex.Sets)
            .SetValidator(new PostPutSetRequestValidator());
    }
}