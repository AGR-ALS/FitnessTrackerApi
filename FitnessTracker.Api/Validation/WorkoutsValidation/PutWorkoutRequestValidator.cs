using FitnessTrackerApi.Contracts.WorkoutsContracts;
using FluentValidation;

namespace FitnessTrackerApi.Validation.WorkoutsValidation;

public class PutWorkoutRequestValidator : AbstractValidator<PutWorkoutRequest>
{
    public PutWorkoutRequestValidator()
    {
        Include(new PostPutWorkoutRequestValidator());
    }
}