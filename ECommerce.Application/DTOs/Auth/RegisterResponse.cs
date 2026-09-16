namespace ECommerce.Application.DTOs.Auth;

public class RegisterResponse
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}