namespace ECommerce.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendVerificationEmailAsync(
        string email,
        string name,
        string verificationLink);

    Task SendPasswordResetEmailAsync(
    string email,
    string name,
    string resetLink);
}