using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> entity)
    {
        entity.HasKey(x => x.Id);

        entity.Property(x => x.UserId)
            .IsRequired();

        entity.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        entity.Property(x => x.AddressLine1)
            .IsRequired()
            .HasMaxLength(200);

        entity.Property(x => x.AddressLine2)
            .HasMaxLength(200);

        entity.Property(x => x.City)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.State)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.PostalCode)
            .IsRequired()
            .HasMaxLength(20);

        entity.Property(x => x.Country)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.IsDefault)
            .IsRequired();

        entity.Property(x => x.CreatedAt)
            .IsRequired();

        entity.Property(x => x.UpdatedAt)
            .IsRequired(false);

        entity.HasOne(x => x.User)
            .WithMany(x => x.Addresses)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(x => x.UserId);
    }
}