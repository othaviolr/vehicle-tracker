using FluentValidation;
using VehicleTracker.Application.DTOs.Location;

namespace VehicleTracker.Application.Validators;

public class CreateLocationValidator : AbstractValidator<CreateLocationDto>
{
    public CreateLocationValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90 degrees");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180 degrees");

        RuleFor(x => x.Speed)
            .GreaterThanOrEqualTo(0).WithMessage("Speed cannot be negative")
            .LessThanOrEqualTo(300).WithMessage("Speed cannot exceed 300 km/h")
            .When(x => x.Speed.HasValue);
    }
}