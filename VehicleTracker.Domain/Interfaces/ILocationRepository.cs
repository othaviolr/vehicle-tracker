using VehicleTracker.Domain.Entities;

namespace VehicleTracker.Domain.Interfaces;

public interface ILocationRepository : IRepository<Location>
{
    Task<IEnumerable<Location>> GetByVehicleIdAsync(Guid vehicleId);
    Task<Location?> GetLatestByVehicleIdAsync(Guid vehicleId);
    Task<(IEnumerable<Location> Items, int TotalCount)> GetPagedByVehicleIdAsync(
        Guid vehicleId,
        int page,
        int pageSize,
        DateTime? startDate = null,
        DateTime? endDate = null);
}