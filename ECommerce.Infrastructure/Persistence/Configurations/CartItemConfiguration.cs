using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> entity)
    {
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Quantity)
            .IsRequired();

        entity.HasOne(x => x.Cart)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasIndex(x => new
        {
            x.CartId,
            x.ProductId
        })
        .IsUnique();
    }
}