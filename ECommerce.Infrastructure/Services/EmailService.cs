using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace ECommerce.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(
        IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendVerificationEmailAsync(
        string email,
        string name,
        string verificationLink)
    {
        var subject = "Verify your email address";

        var body = $"""
            <!DOCTYPE html>
            <html>
            <body>
                <h2>Verify Your Email</h2>

                <p>Hello {WebUtility.HtmlEncode(name)},</p>

                <p>
                    Thank you for registering.
                    Please click the button below to verify your email address.
                </p>

                <p>
                    <a href="{verificationLink}"
                       style="background-color:#007bff;
                              color:white;
                              padding:10px 20px;
                              text-decoration:none;
                              border-radius:5px;">
                        Verify Email
                    </a>
                </p>

                <p>
                    This verification link will expire in 24 hours.
                </p>

                <p>
                    If you did not create this account, you can safely ignore
                    this email.
                </p>
            </body>
            </html>
            """;

        await SendEmailAsync(
            email,
            subject,
            body);
    }

    public async Task SendPasswordResetEmailAsync(
        string email,
        string name,
        string resetLink)
    {
        var subject = "Reset your password";

        var body = $"""
            <!DOCTYPE html>
            <html>
            <body>
                <h2>Password Reset</h2>

                <p>Hello {WebUtility.HtmlEncode(name)},</p>

                <p>
                    We received a request to reset your password.
                </p>

                <p>
                    Click the button below to reset your password.
                </p>

                <p>
                    <a href="{resetLink}"
                       style="background-color:#007bff;
                              color:white;
                              padding:10px 20px;
                              text-decoration:none;
                              border-radius:5px;">
                        Reset Password
                    </a>
                </p>

                <p>
                    This password reset link will expire in 30 minutes.
                </p>

                <p>
                    If you did not request a password reset,
                    you can safely ignore this email.
                </p>
            </body>
            </html>
            """;

        await SendEmailAsync(
            email,
            subject,
            body);
    }

    private async Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlBody)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(
                _settings.FromEmail,
                _settings.FromName),

            Subject = subject,

            Body = htmlBody,

            IsBodyHtml = true
        };

        message.To.Add(toEmail);

        using var smtpClient = new SmtpClient(
            _settings.Host,
            _settings.Port)
        {
            Credentials = new NetworkCredential(
                _settings.Username,
                _settings.Password),

            EnableSsl = true
        };

        await smtpClient.SendMailAsync(message);
    }
}