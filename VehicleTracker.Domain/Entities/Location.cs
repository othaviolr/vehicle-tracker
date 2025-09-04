namespace VehicleTracker.Domain.Entities;

public class Location : BaseEntity
{
    public Guid VehicleId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Speed { get; set; }
    public DateTime RecordedAt { get; set; }
    public virtual Vehicle Vehicle { get; set; } = null!;
}