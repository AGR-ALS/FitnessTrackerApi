using System.Text.RegularExpressions;
using FitnessTrackerApi.Contracts.WorkoutsContracts;
using FluentValidation;

namespace FitnessTrackerApi.Validation.WorkoutsValidation;

public class GetWorkoutRequestValidation : AbstractValidator<GetWorkoutRequest>
{
    public GetWorkoutRequestValidation()
    {
        RuleFor(w=> w.Duration)
            .GreaterThan(TimeSpan.Zero)
            .LessThanOrEqualTo(TimeSpan.FromDays(1)).WithMessage("Duration must be between 00:00:01 and 1.00:00:00");
        RuleFor(w => w.SortItem)
            .Matches("^(?i)(Id|CreatedAt|CaloriesBurned)?$").WithMessage("Invalid sort item");
        RuleFor(w=> w.SortOrder)
            .Matches("^(?i)(asc|desc)?$").WithMessage("SortOrder must be 'asc' or 'desc'");
        RuleFor(w=>w.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0");
        RuleFor(w=>w.PageSize)
            .GreaterThan(1)
            .LessThanOrEqualTo(100).WithMessage("PageSize must be between 1 and 100");
    }
}