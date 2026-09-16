namespace ECommerce.Application.DTOs.Auth;

public class LoginResponse
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string AccessToken { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
}