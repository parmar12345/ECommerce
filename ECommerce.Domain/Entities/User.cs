namespace ECommerce.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public bool IsEmailVerified { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int age { get; set; }

    public ICollection<PasswordResetToken> PasswordResetTokens
    { get; set; } = new List<PasswordResetToken>();

    public ICollection<Address> Addresses
    { get; set; } = new List<Address>();
}