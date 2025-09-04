using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleTracker.Domain.Entities;
using VehicleTracker.Domain.Enums;

namespace VehicleTracker.Infrastructure.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.VehicleId)
            .IsRequired()
            .HasComment("Reference to vehicle");

        builder.Property(l => l.Latitude)
            .IsRequired()
            .HasPrecision(10, 7)
            .HasComment("GPS latitude coordinate");

        builder.Property(l => l.Longitude)
            .IsRequired()
            .HasPrecision(10, 7)
            .HasComment("GPS longitude coordinate");

        builder.Property(l => l.Speed)
            .HasPrecision(5, 2)
            .HasComment("Vehicle speed in km/h");

        builder.Property(l => l.RecordedAt)
            .IsRequired()
            .HasComment("When the location was recorded");

        builder.Property(l => l.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Record creation timestamp");

        builder.Property(l => l.UpdatedAt)
            .HasComment("Record last update timestamp");

        builder.Property(l => l.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Soft delete flag");

        builder.HasIndex(l => l.VehicleId)
            .HasDatabaseName("IX_Locations_VehicleId");

        builder.HasIndex(l => l.RecordedAt)
            .HasDatabaseName("IX_Locations_RecordedAt");

        builder.HasIndex(l => new { l.VehicleId, l.RecordedAt })
            .HasDatabaseName("IX_Locations_VehicleId_RecordedAt");
    }
}