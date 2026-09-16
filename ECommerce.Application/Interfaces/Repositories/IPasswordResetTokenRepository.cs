using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken> CreateAsync(
        PasswordResetToken token);

    Task<PasswordResetToken?> GetByTokenHashAsync(
        string tokenHash);

    Task DeleteByUserIdAsync(
        Guid userId);

    Task MarkAsUsedAsync(
        PasswordResetToken token);
}