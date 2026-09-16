namespace ECommerce.Application.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateAccessToken(
        Guid userId,
        string email,
        string name);

    DateTime GetExpirationTime();
}