using System.Security.Cryptography;
using System.Text;
using ECommerce.Application.Interfaces.Services;

namespace ECommerce.Infrastructure.Services;

public class RefreshTokenService
    : IRefreshTokenService
{
    public string GenerateToken()
    {
        var bytes =
            RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(bytes);
    }

    public string HashToken(string token)
    {
        var bytes =
            SHA256.HashData(
                Encoding.UTF8.GetBytes(token));

        return Convert.ToHexString(bytes)
            .ToLowerInvariant();
    }
}