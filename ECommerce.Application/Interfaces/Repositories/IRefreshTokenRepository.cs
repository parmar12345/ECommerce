using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> CreateAsync(
        RefreshToken refreshToken);

    Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash);

    Task RevokeAsync(
        RefreshToken refreshToken);

    Task RevokeAllByUserIdAsync(
    Guid userId);
}