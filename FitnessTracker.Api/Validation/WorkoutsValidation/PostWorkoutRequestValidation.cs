using FitnessTrackerApi.Contracts.WorkoutsContracts;
using FluentValidation;

namespace FitnessTrackerApi.Validation.WorkoutsValidation;

public class PostWorkoutRequestValidation : AbstractValidator<PostWorkoutRequest>
{
    public PostWorkoutRequestValidation()
    {
        Include(new PostPutWorkoutRequestValidator());
    }
}