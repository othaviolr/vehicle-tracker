using VehicleTracker.Application.DTOs;
using VehicleTracker.Application.DTOs.Location;

namespace VehicleTracker.Application.Interfaces;

public interface ILocationService
{
    Task<LocationResponseDto?> GetByIdAsync(Guid id);
    Task<PagedResponse<LocationResponseDto>> GetPagedByVehicleIdAsync(
        Guid vehicleId,
        int page,
        int pageSize,
        DateTime? startDate = null,
        DateTime? endDate = null);
    Task<LocationResponseDto?> GetLatestByVehicleIdAsync(Guid vehicleId);
    Task<LocationResponseDto> CreateAsync(Guid vehicleId, CreateLocationDto dto);
}