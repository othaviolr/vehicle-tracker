using Microsoft.EntityFrameworkCore;
using VehicleTracker.Domain.Entities;
using VehicleTracker.Domain.Interfaces;
using VehicleTracker.Infrastructure.Data;

namespace VehicleTracker.Infrastructure.Repositories;

public class LocationRepository : Repository<Location>, ILocationRepository
{
    public LocationRepository(VehicleTrackerDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Location>> GetByVehicleIdAsync(Guid vehicleId)
    {
        return await _dbSet
            .Where(l => l.VehicleId == vehicleId)
            .OrderByDescending(l => l.RecordedAt)
            .ToListAsync();
    }

    public async Task<Location?> GetLatestByVehicleIdAsync(Guid vehicleId)
    {
        return await _dbSet
            .Where(l => l.VehicleId == vehicleId)
            .OrderByDescending(l => l.RecordedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<(IEnumerable<Location> Items, int TotalCount)> GetPagedByVehicleIdAsync(
        Guid vehicleId,
        int page,
        int pageSize,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = _dbSet.Where(l => l.VehicleId == vehicleId);

        if (startDate.HasValue)
        {
            query = query.Where(l => l.RecordedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(l => l.RecordedAt <= endDate.Value);
        }

        query = query.OrderByDescending(l => l.RecordedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}