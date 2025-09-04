using VehicleTracker.Domain.Enums;

namespace VehicleTracker.Domain.Entities;

public class Vehicle : BaseEntity
{
    public string Plate { get; set; } = string.Empty;
    public VehicleBrand Brand { get; set; }
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public VehicleStatus Status { get; set; }
    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
}