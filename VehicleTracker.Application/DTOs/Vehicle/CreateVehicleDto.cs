using VehicleTracker.Domain.Enums;

namespace VehicleTracker.Application.DTOs.Vehicle;

public class CreateVehicleDto
{
    public string Plate { get; set; } = string.Empty;
    public VehicleBrand Brand { get; set; }
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
}