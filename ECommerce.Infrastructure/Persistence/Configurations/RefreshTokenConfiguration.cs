using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity.HasKey(x => x.Id);

        entity.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(64);

        entity.HasIndex(x => x.TokenHash)
            .IsUnique();

        entity.Property(x => x.ExpiresAt)
            .IsRequired();

        entity.Property(x => x.CreatedAt)
            .IsRequired();

        entity.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}