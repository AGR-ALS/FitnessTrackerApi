using FitnessTrackerApi.Contracts.WorkoutsContracts;
using FluentValidation;

namespace FitnessTrackerApi.Validation.WorkoutsValidation;

public class IdQueryParameterValidator : AbstractValidator<string>
{
    public IdQueryParameterValidator()
    {
        RuleFor(i=>i)
            .NotEmpty().WithMessage("Id is required")
            .Length(36).WithMessage("Id must be 36 characters long")
            .Matches("^[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}$").WithMessage("Invalid workout id");
    }
}