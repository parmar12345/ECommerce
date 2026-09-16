using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class EmailVerificationTokenConfiguration
    : IEntityTypeConfiguration<EmailVerificationToken>
{
    public void Configure(EntityTypeBuilder<EmailVerificationToken> entity)
    {
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(500);

        entity.HasIndex(x => x.Token)
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