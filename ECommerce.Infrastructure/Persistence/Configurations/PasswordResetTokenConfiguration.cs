using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class PasswordResetTokenConfiguration
    : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> entity)
    {
        entity.HasKey(x => x.Id);

        entity.Property(x => x.TokenHash)
            .IsRequired()
            .HasMaxLength(64);

        entity.HasIndex(x => x.TokenHash)
            .IsUnique();

        entity.Property(x => x.CreatedAt)
            .IsRequired();

        entity.Property(x => x.ExpiresAt)
            .IsRequired();

        entity.HasOne(x => x.User)
            .WithMany(x => x.PasswordResetTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}