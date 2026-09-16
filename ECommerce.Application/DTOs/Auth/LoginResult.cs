
namespace ECommerce.Application.DTOs.Auth;

public class LoginResult
{
    public LoginResponse Response { get; set; } = null!;

    public string RefreshToken { get; set; } = string.Empty;
}