using FluentValidation;
using VehicleTracker.Application.DTOs.Vehicle;
using VehicleTracker.Domain.Enums;

namespace VehicleTracker.Application.Validators;

public class UpdateVehicleStatusValidator : AbstractValidator<UpdateVehicleStatusDto>
{
    public UpdateVehicleStatusValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid vehicle status");
    }
}