namespace ECommerce.Domain.Entities;

public class PasswordResetToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime? UsedAt { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}