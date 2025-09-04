using VehicleTracker.Domain.Enums;

namespace VehicleTracker.Application.DTOs.Vehicle;

public class UpdateVehicleStatusDto
{
    public VehicleStatus Status { get; set; }
}