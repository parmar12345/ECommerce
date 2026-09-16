using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> entity)
    {
        entity.HasKey(x => x.Id);

        entity.Property(x => x.UserId)
            .IsRequired();

        entity.Property(x => x.CreatedAt)
            .IsRequired();

        entity.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<Cart>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex(x => x.UserId)
            .IsUnique();
    }
}