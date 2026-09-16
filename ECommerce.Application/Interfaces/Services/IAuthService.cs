using ECommerce.Application.DTOs.Auth;

namespace ECommerce.Application.Interfaces.Services;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(
        RegisterRequest request);

    Task<LoginResult> LoginAsync(
        LoginRequest request);

    Task<LoginResult> RefreshTokenAsync(
        string refreshToken);

    Task LogoutAsync(string refreshToken);

    Task<GetMeResponse> GetMeAsync(Guid userId);

    Task ForgotPasswordAsync(ForgotPasswordRequest request);

    Task ResetPasswordAsync(ResetPasswordRequest request);
}