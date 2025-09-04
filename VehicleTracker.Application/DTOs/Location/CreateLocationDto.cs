namespace VehicleTracker.Application.DTOs.Location;

public class CreateLocationDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? Speed { get; set; }
}