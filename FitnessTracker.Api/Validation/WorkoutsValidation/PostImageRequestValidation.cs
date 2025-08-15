using FitnessTrackerApi.Contracts.WorkoutsContracts;
using FluentValidation;

namespace FitnessTrackerApi.Validation.WorkoutsValidation;

public class PostImageRequestValidation : AbstractValidator<PostImagesRequest>
{
    public PostImageRequestValidation()
    {
        RuleFor(i => i.ProgressPhotos)
            .NotEmpty().WithMessage("Photos are required for uploading");
    }
}