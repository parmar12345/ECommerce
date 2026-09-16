using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IEmailVerificationTokenRepository
{
    Task<EmailVerificationToken> CreateAsync(
        EmailVerificationToken token);

    Task<EmailVerificationToken?> GetByTokenAsync(
        string token);

    Task MarkAsUsedAsync(
        EmailVerificationToken token);

    Task DeleteByUserIdAsync(
        Guid userId);
}