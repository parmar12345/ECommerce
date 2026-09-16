namespace ECommerce.Application.Interfaces.Services;

public interface IRefreshTokenService
{
    string GenerateToken();

    string HashToken(string token);
}