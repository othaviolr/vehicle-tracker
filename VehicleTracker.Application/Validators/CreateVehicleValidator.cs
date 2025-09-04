using FluentValidation;
using VehicleTracker.Application.DTOs.Vehicle;
using VehicleTracker.Domain.Enums;

namespace VehicleTracker.Application.Validators;

public class CreateVehicleValidator : AbstractValidator<CreateVehicleDto>
{
    public CreateVehicleValidator()
    {
        RuleFor(x => x.Plate)
            .NotEmpty().WithMessage("Plate is required")
            .Matches(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$|^[A-Z]{3}-?[0-9]{4}$")
            .WithMessage("Plate must be in valid Brazilian format (ABC-1234 or ABC1D23)");

        RuleFor(x => x.Brand)
            .IsInEnum().WithMessage("Invalid vehicle brand");

        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required")
            .MaximumLength(100).WithMessage("Model cannot exceed 100 characters");

        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo(1980).WithMessage("Year must be 1980 or later")
            .LessThanOrEqualTo(DateTime.Now.Year + 1).WithMessage($"Year cannot be greater than {DateTime.Now.Year + 1}");
    }
}