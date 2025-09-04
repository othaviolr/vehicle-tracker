using VehicleTracker.Application.DTOs;
using VehicleTracker.Application.DTOs.Vehicle;
using VehicleTracker.Application.Interfaces;
using VehicleTracker.Domain.Entities;
using VehicleTracker.Domain.Enums;
using VehicleTracker.Domain.Interfaces;
using VehicleTracker.Infrastructure.Data;

namespace VehicleTracker.Application.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly VehicleTrackerDbContext _context;

    public VehicleService(
        IVehicleRepository vehicleRepository,
        ILocationRepository locationRepository,
        VehicleTrackerDbContext context)
    {
        _vehicleRepository = vehicleRepository;
        _locationRepository = locationRepository;
        _context = context;
    }

    public async Task<VehicleResponseDto?> GetByIdAsync(Guid id)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        return vehicle == null ? null : await MapToResponseDto(vehicle);
    }

    public async Task<VehicleResponseDto?> GetByPlateAsync(string plate)
    {
        var vehicle = await _vehicleRepository.GetByPlateAsync(plate);
        return vehicle == null ? null : await MapToResponseDto(vehicle);
    }

    public async Task<PagedResponse<VehicleResponseDto>> GetPagedAsync(int page, int pageSize)
    {
        var (vehicles, totalCount) = await _vehicleRepository.GetPagedAsync(page, pageSize);

        var vehicleDtos = new List<VehicleResponseDto>();
        foreach (var vehicle in vehicles)
        {
            vehicleDtos.Add(await MapToResponseDto(vehicle));
        }

        return new PagedResponse<VehicleResponseDto>
        {
            Items = vehicleDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<VehicleResponseDto>> GetByStatusAsync(VehicleStatus status)
    {
        var vehicles = await _vehicleRepository.GetByStatusAsync(status);

        var vehicleDtos = new List<VehicleResponseDto>();
        foreach (var vehicle in vehicles)
        {
            vehicleDtos.Add(await MapToResponseDto(vehicle));
        }

        return vehicleDtos;
    }

    public async Task<VehicleResponseDto> CreateAsync(CreateVehicleDto dto)
    {
        if (await _vehicleRepository.PlateExistsAsync(dto.Plate))
        {
            throw new InvalidOperationException($"Vehicle with plate {dto.Plate} already exists");
        }

        var vehicle = new Vehicle
        {
            Plate = dto.Plate.ToUpper(),
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            Status = VehicleStatus.Active
        };

        await _vehicleRepository.AddAsync(vehicle);
        await _context.SaveChangesAsync();
        return await MapToResponseDto(vehicle);
    }

    public async Task<VehicleResponseDto?> UpdateStatusAsync(Guid id, UpdateVehicleStatusDto dto)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(id);
        if (vehicle == null) return null;

        vehicle.Status = dto.Status;
        vehicle.UpdatedAt = DateTime.UtcNow;

        await _vehicleRepository.UpdateAsync(vehicle);
        await _context.SaveChangesAsync();
        return await MapToResponseDto(vehicle);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        if (!await _vehicleRepository.ExistsAsync(id)) return false;

        await _vehicleRepository.DeleteAsync(id);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _vehicleRepository.ExistsAsync(id);
    }

    private async Task<VehicleResponseDto> MapToResponseDto(Vehicle vehicle)
    {
        var lastLocation = await _locationRepository.GetLatestByVehicleIdAsync(vehicle.Id);

        return new VehicleResponseDto
        {
            Id = vehicle.Id,
            Plate = vehicle.Plate,
            Brand = vehicle.Brand,
            BrandName = vehicle.Brand.ToString(),
            Model = vehicle.Model,
            Year = vehicle.Year,
            Status = vehicle.Status,
            StatusName = vehicle.Status.ToString(),
            CreatedAt = vehicle.CreatedAt,
            LastLocationAt = lastLocation?.RecordedAt
        };
    }
}