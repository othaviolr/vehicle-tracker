using Microsoft.EntityFrameworkCore;
using VehicleTracker.Domain.Entities;
using VehicleTracker.Domain.Enums;
using VehicleTracker.Domain.Interfaces;
using VehicleTracker.Infrastructure.Data;

namespace VehicleTracker.Infrastructure.Repositories;

public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(VehicleTrackerDbContext context) : base(context)
    {
    }

    public async Task<Vehicle?> GetByPlateAsync(string plate)
    {
        return await _dbSet.FirstOrDefaultAsync(v => v.Plate == plate.ToUpper());
    }

    public async Task<IEnumerable<Vehicle>> GetByStatusAsync(VehicleStatus status)
    {
        return await _dbSet.Where(v => v.Status == status).ToListAsync();
    }

    public async Task<(IEnumerable<Vehicle> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        var query = _dbSet.OrderBy(v => v.CreatedAt);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> PlateExistsAsync(string plate, Guid? excludeId = null)
    {
        var query = _dbSet.Where(v => v.Plate == plate.ToUpper());

        if (excludeId.HasValue)
        {
            query = query.Where(v => v.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}