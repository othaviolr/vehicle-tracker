using VehicleTracker.Domain.Entities;
using VehicleTracker.Domain.Enums;

namespace VehicleTracker.Domain.Interfaces;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<Vehicle?> GetByPlateAsync(string plate);
    Task<IEnumerable<Vehicle>> GetByStatusAsync(VehicleStatus status);
    Task<(IEnumerable<Vehicle> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    Task<bool> PlateExistsAsync(string plate, Guid? excludeId = null);
}