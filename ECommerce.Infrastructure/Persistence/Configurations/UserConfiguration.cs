using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

        entity.HasIndex(x => x.Email)
            .IsUnique();

        entity.Property(x => x.PasswordHash)
            .IsRequired();

        entity.Property(x => x.IsEmailVerified)
            .IsRequired();

        entity.Property(x => x.CreatedAt)
            .IsRequired();
    }
}