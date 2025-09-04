using VehicleTracker.Application.DTOs;
using VehicleTracker.Application.DTOs.Location;
using VehicleTracker.Application.Interfaces;
using VehicleTracker.Domain.Entities;
using VehicleTracker.Domain.Interfaces;
using VehicleTracker.Infrastructure.Data;

namespace VehicleTracker.Application.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly VehicleTrackerDbContext _context;

    public LocationService(
        ILocationRepository locationRepository,
        IVehicleRepository vehicleRepository,
        VehicleTrackerDbContext context)
    {
        _locationRepository = locationRepository;
        _vehicleRepository = vehicleRepository;
        _context = context;
    }

    public async Task<LocationResponseDto?> GetByIdAsync(Guid id)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        return location == null ? null : MapToResponseDto(location);
    }

    public async Task<PagedResponse<LocationResponseDto>> GetPagedByVehicleIdAsync(
        Guid vehicleId,
        int page,
        int pageSize,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var (locations, totalCount) = await _locationRepository.GetPagedByVehicleIdAsync(
            vehicleId, page, pageSize, startDate, endDate);

        var locationDtos = locations.Select(MapToResponseDto);

        return new PagedResponse<LocationResponseDto>
        {
            Items = locationDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<LocationResponseDto?> GetLatestByVehicleIdAsync(Guid vehicleId)
    {
        var location = await _locationRepository.GetLatestByVehicleIdAsync(vehicleId);
        return location == null ? null : MapToResponseDto(location);
    }

    public async Task<LocationResponseDto> CreateAsync(Guid vehicleId, CreateLocationDto dto)
    {
        if (!await _vehicleRepository.ExistsAsync(vehicleId))
        {
            throw new InvalidOperationException($"Vehicle with ID {vehicleId} not found");
        }

        var location = new Location
        {
            VehicleId = vehicleId,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Speed = dto.Speed,
            RecordedAt = DateTime.UtcNow
        };

        await _locationRepository.AddAsync(location);
        await _context.SaveChangesAsync();
        return MapToResponseDto(location);
    }

    private static LocationResponseDto MapToResponseDto(Location location)
    {
        return new LocationResponseDto
        {
            Id = location.Id,
            VehicleId = location.VehicleId,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Speed = location.Speed,
            RecordedAt = location.RecordedAt,
            CreatedAt = location.CreatedAt
        };
    }
}