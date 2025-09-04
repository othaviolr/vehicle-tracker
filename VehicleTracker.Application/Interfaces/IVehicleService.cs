using VehicleTracker.Application.DTOs;
using VehicleTracker.Application.DTOs.Vehicle;
using VehicleTracker.Domain.Enums;

namespace VehicleTracker.Application.Interfaces;

public interface IVehicleService
{
    Task<VehicleResponseDto?> GetByIdAsync(Guid id);
    Task<VehicleResponseDto?> GetByPlateAsync(string plate);
    Task<PagedResponse<VehicleResponseDto>> GetPagedAsync(int page, int pageSize);
    Task<IEnumerable<VehicleResponseDto>> GetByStatusAsync(VehicleStatus status);
    Task<VehicleResponseDto> CreateAsync(CreateVehicleDto dto);
    Task<VehicleResponseDto?> UpdateStatusAsync(Guid id, UpdateVehicleStatusDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}