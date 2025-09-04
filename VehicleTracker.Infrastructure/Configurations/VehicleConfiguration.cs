using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleTracker.Domain.Entities;
using VehicleTracker.Domain.Enums;

namespace VehicleTracker.Infrastructure.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicles");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Plate)
            .IsRequired()
            .HasMaxLength(8)
            .HasComment("Vehicle plate number");

        builder.HasIndex(v => v.Plate)
            .IsUnique()
            .HasDatabaseName("IX_Vehicles_Plate");

        builder.Property(v => v.Brand)
            .IsRequired()
            .HasConversion<int>()
            .HasComment("Vehicle brand enum");

        builder.Property(v => v.Model)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Vehicle model");

        builder.Property(v => v.Year)
            .IsRequired()
            .HasComment("Vehicle manufacturing year");

        builder.Property(v => v.Status)
            .IsRequired()
            .HasConversion<int>()
            .HasDefaultValue(VehicleStatus.Active)
            .HasComment("Current vehicle status");

        builder.Property(v => v.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Record creation timestamp");

        builder.Property(v => v.UpdatedAt)
            .HasComment("Record last update timestamp");

        builder.Property(v => v.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false)
            .HasComment("Soft delete flag");

        builder.HasMany(v => v.Locations)
            .WithOne(l => l.Vehicle)
            .HasForeignKey(l => l.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}