namespace ECommerce.Application.Interfaces.Services;

public interface IEmailVerificationService
{
    Task GenerateVerificationTokenAsync(Guid userId);

    Task VerifyEmailAsync(string token);

    Task ResendVerificationEmailAsync(string email);
}