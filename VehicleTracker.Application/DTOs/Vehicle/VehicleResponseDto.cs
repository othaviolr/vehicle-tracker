using VehicleTracker.Domain.Enums;

namespace VehicleTracker.Application.DTOs.Vehicle;

public class VehicleResponseDto
{
    public Guid Id { get; set; }
    public string Plate { get; set; } = string.Empty;
    public VehicleBrand Brand { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public VehicleStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLocationAt { get; set; }
}