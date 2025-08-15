using FitnessTrackerApi.Contracts.WorkoutsContracts;
using FluentValidation;

namespace FitnessTrackerApi.Validation.WorkoutsValidation;

public class PostPutWorkoutRequestValidator : AbstractValidator<PostPutWorkoutRequest>
{
    public PostPutWorkoutRequestValidator()
    {
        RuleFor(x => x.CreatedAt)
            .NotEmpty().WithMessage("CreatedAt is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title length must be less than 100 characters");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid workout type");

        RuleFor(x => x.Exercises)
            .NotEmpty().WithMessage("Exercises are required");

        RuleForEach(x => x.Exercises)
            .SetValidator(new PostPutExerciseRequestValidator());

        RuleFor(x => x.Duration)
            .GreaterThan(TimeSpan.Zero)
            .LessThanOrEqualTo(TimeSpan.FromDays(1)).WithMessage("Duration must be between 00:00:01 and 1.00:00:00");

        RuleFor(x => x.CaloriesBurned)
            .GreaterThanOrEqualTo(0).WithMessage("CaloriesBurned cannot be negative");

        RuleFor(x => x.WorkoutDate)
            .NotEmpty().WithMessage("WorkoutDate is required");
    }
}