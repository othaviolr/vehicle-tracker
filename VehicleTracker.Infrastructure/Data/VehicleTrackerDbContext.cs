using Microsoft.EntityFrameworkCore;
using VehicleTracker.Domain.Entities;
using VehicleTracker.Domain.Enums;
using VehicleTracker.Infrastructure.Configurations;

namespace VehicleTracker.Infrastructure.Data;

public class VehicleTrackerDbContext : DbContext
{
    public VehicleTrackerDbContext(DbContextOptions<VehicleTrackerDbContext> options) : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Location> Locations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração Vehicle
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.ToTable("Vehicles");
            entity.HasKey(v => v.Id);

            entity.Property(v => v.Plate)
                .IsRequired()
                .HasMaxLength(8);

            entity.HasIndex(v => v.Plate).IsUnique();

            entity.Property(v => v.Brand)
                .IsRequired()
                .HasConversion<int>();

            entity.Property(v => v.Model)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(v => v.Status)
                .IsRequired()
                .HasConversion<int>()
                .HasDefaultValue(VehicleStatus.Active);

            entity.Property(v => v.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(v => v.IsDeleted)
                .HasDefaultValue(false);

            // Relacionamento
            entity.HasMany(v => v.Locations)
                .WithOne(l => l.Vehicle)
                .HasForeignKey(l => l.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração Location
        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("Locations");
            entity.HasKey(l => l.Id);

            entity.Property(l => l.VehicleId).IsRequired();
            entity.Property(l => l.Latitude).IsRequired().HasPrecision(10, 7);
            entity.Property(l => l.Longitude).IsRequired().HasPrecision(10, 7);
            entity.Property(l => l.Speed).HasPrecision(5, 2);
            entity.Property(l => l.RecordedAt).IsRequired();
            entity.Property(l => l.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(l => l.IsDeleted).HasDefaultValue(false);

            // Índices
            entity.HasIndex(l => l.VehicleId);
            entity.HasIndex(l => l.RecordedAt);
            entity.HasIndex(l => new { l.VehicleId, l.RecordedAt });
        });

        // Soft delete global
        modelBuilder.Entity<Vehicle>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Location>().HasQueryFilter(e => !e.IsDeleted);
    }
}