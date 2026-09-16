using System.Security.Cryptography;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class EmailVerificationService : IEmailVerificationService
{
    private readonly IEmailVerificationTokenRepository
        _tokenRepository;

    private readonly IUserRepository
        _userRepository;

    private readonly IEmailService
        _emailService;

    public EmailVerificationService(
        IEmailVerificationTokenRepository tokenRepository,
        IUserRepository userRepository,
        IEmailService emailService)
    {
        _tokenRepository = tokenRepository;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task GenerateVerificationTokenAsync(
        Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new InvalidOperationException(
                "User not found.");
        }

        await _tokenRepository.DeleteByUserIdAsync(userId);

        var tokenBytes =
            RandomNumberGenerator.GetBytes(32);

        var token =
            Convert.ToBase64String(tokenBytes);

        var verificationToken = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        await _tokenRepository.CreateAsync(
            verificationToken);

        var verificationLink =
     $"http://localhost:4200/auth/verify-email?token={Uri.EscapeDataString(token)}";

        await _emailService.SendVerificationEmailAsync(
            user.Email,
            user.Name,
            verificationLink);
    }

    public async Task VerifyEmailAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException(
                "Verification token is required.");
        }

        var verificationToken =
            await _tokenRepository.GetByTokenAsync(token);

        if (verificationToken == null)
        {
            throw new InvalidOperationException(
                "Invalid verification token.");
        }

        if (verificationToken.UsedAt.HasValue)
        {
            throw new InvalidOperationException(
                "This verification token has already been used.");
        }

        if (verificationToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "This verification token has expired.");
        }

        await _userRepository.MarkEmailAsVerifiedAsync(
            verificationToken.UserId);

        await _tokenRepository.MarkAsUsedAsync(
            verificationToken);
    }

    public async Task ResendVerificationEmailAsync(
    string email)
    {
        var normalizedEmail =
            email.Trim().ToLowerInvariant();

        var user =
            await _userRepository.GetByEmailAsync(
                normalizedEmail);

        if (user == null)
        {
            throw new InvalidOperationException(
                "Unable to process email verification request.");
        }

        if (user.IsEmailVerified)
        {
            throw new InvalidOperationException(
                "This email address is already verified.");
        }

        await GenerateVerificationTokenAsync(user.Id);
    }
}