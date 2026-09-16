using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Exceptions;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Validators.Auth;
using ECommerce.Domain.Entities;
using FluentValidation;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IValidator<RegisterRequest> _validator;
    private readonly IEmailVerificationService _emailVerificationService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IValidator<LoginRequest> _loginValidator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IValidator<ForgotPasswordRequest> _forgotPasswordValidator;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IEmailService _emailService;
    private readonly IValidator<ResetPasswordRequest> _resetPasswordValidator;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IValidator<RegisterRequest> validator,
        IValidator<LoginRequest> loginValidator,
        IEmailVerificationService emailVerificationService,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenService refreshTokenService,
        IValidator<ForgotPasswordRequest> forgotPasswordValidator,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IValidator<ResetPasswordRequest> resetPasswordValidator,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _validator = validator;
        _loginValidator = loginValidator;
        _emailVerificationService = emailVerificationService;
        _jwtTokenService = jwtTokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenService = refreshTokenService;
        _forgotPasswordValidator = forgotPasswordValidator;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _resetPasswordValidator = resetPasswordValidator;
        _emailService = emailService;
        
    }

    public async Task<RegisterResponse> RegisterAsync(
        RegisterRequest request)
    {
        // 1. Validate request
        await _validator.ValidateAndThrowAsync(request);

        // 2. Normalize email
        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        // 3. Check duplicate email
        var emailExists =
            await _userRepository.ExistsByEmailAsync(email);

        if (emailExists)
        {
            throw new ConflictException(
                "An account with this email already exists.");
        }

        // 4. Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            IsEmailVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        // 5. Save user
        var createdUser =
            await _userRepository.CreateAsync(user);

        // 6. Generate verification token
        await _emailVerificationService
            .GenerateVerificationTokenAsync(createdUser.Id);

        // 7. Return response
        return new RegisterResponse
        {
            UserId = createdUser.Id,
            Name = createdUser.Name,
            Email = createdUser.Email,
            Message = "Registration successful. Please verify your email."
        };
    }

    public async Task<LoginResult>LoginAsync(LoginRequest request)
    {
        await _loginValidator
            .ValidateAndThrowAsync(request);

        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        var user =
            await _userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        var passwordValid =
            _passwordHasher.Verify(
                request.Password,
                user.PasswordHash);

        if (!passwordValid)
        {
            throw new UnauthorizedAccessException(
                "Invalid email or password.");
        }

        if (!user.IsEmailVerified)
        {
            throw new UnauthorizedAccessException(
                "Please verify your email before logging in.");
        }

        var accessToken =
            _jwtTokenService.GenerateAccessToken(
                user.Id,
                user.Email,
                user.Name);

        var expiresAt =
            _jwtTokenService.GetExpirationTime();

        // Generate refresh token
        var refreshToken =
            _refreshTokenService.GenerateToken();

        var refreshTokenHash =
            _refreshTokenService.HashToken(
                refreshToken);

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt =
                DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepository.CreateAsync(
            refreshTokenEntity);

        var response = new LoginResponse
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            AccessToken = accessToken,
            ExpiresAt = expiresAt
        };

        return new LoginResult
        {
            Response = response,
            RefreshToken = refreshToken
        };
    }

    public async Task<LoginResult> RefreshTokenAsync(
     string refreshToken)
    {
        // 1. Validate cookie/token
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new UnauthorizedAccessException(
                "Refresh token is required.");
        }

        // 2. Hash incoming refresh token
        var tokenHash =
            _refreshTokenService.HashToken(
                refreshToken);

        // 3. Find token in database
        var existingToken =
            await _refreshTokenRepository
                .GetByTokenHashAsync(tokenHash);

        if (existingToken == null)
        {
            throw new UnauthorizedAccessException(
                "Invalid refresh token.");
        }

        // 4. Detect refresh-token reuse
        if (existingToken.RevokedAt.HasValue)
        {
            // The token was already used/revoked.
            // Revoke all active sessions for this user.
            await _refreshTokenRepository
                .RevokeAllByUserIdAsync(
                    existingToken.UserId);

            throw new UnauthorizedAccessException(
                "Refresh token reuse detected.");
        }

        // 5. Check expiration
        if (existingToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException(
                "Refresh token has expired.");
        }

        // 6. Get user
        var user = existingToken.User;

        if (user == null)
        {
            throw new UnauthorizedAccessException(
                "User associated with refresh token was not found.");
        }

        // 7. Revoke old refresh token
        await _refreshTokenRepository.RevokeAsync(
            existingToken);

        // 8. Generate new access token
        var accessToken =
            _jwtTokenService.GenerateAccessToken(
                user.Id,
                user.Email,
                user.Name);

        var expiresAt =
            _jwtTokenService.GetExpirationTime();

        // 9. Generate new refresh token
        var newRefreshToken =
            _refreshTokenService.GenerateToken();

        // 10. Hash new refresh token
        var newRefreshTokenHash =
            _refreshTokenService.HashToken(
                newRefreshToken);

        // 11. Save new refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = newRefreshTokenHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepository.CreateAsync(
            newRefreshTokenEntity);

        // 12. Return new tokens
        var response = new LoginResponse
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            AccessToken = accessToken,
            ExpiresAt = expiresAt
        };

        return new LoginResult
        {
            Response = response,
            RefreshToken = newRefreshToken
        };
    }

    public async Task LogoutAsync(
    string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }

        var tokenHash =
            _refreshTokenService.HashToken(
                refreshToken);

        var existingToken =
            await _refreshTokenRepository
                .GetByTokenHashAsync(tokenHash);

        if (existingToken == null)
        {
            return;
        }

        if (!existingToken.RevokedAt.HasValue)
        {
            await _refreshTokenRepository.RevokeAsync(
                existingToken);
        }
    }

    public async Task<GetMeResponse> GetMeAsync(
    Guid userId)
    {
        var user =
            await _userRepository.GetByIdAsync(
                userId);

        if (user == null)
        {
            throw new UnauthorizedAccessException(
                "User not found.");
        }

        return new GetMeResponse
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }

    public async Task ForgotPasswordAsync(
    ForgotPasswordRequest request)
    {
        // 1. Validate request
        await _forgotPasswordValidator
            .ValidateAndThrowAsync(request);

        // 2. Normalize email
        var email = request.Email
            .Trim()
            .ToLowerInvariant();

        // 3. Find user
        var user =
            await _userRepository.GetByEmailAsync(email);

        // Important:
        // Do not reveal whether the email exists.
        if (user == null)
        {
            return;
        }

        // 4. Delete previous reset tokens
        await _passwordResetTokenRepository
            .DeleteByUserIdAsync(user.Id);

        // 5. Generate cryptographically secure token
        var tokenBytes =
            RandomNumberGenerator.GetBytes(32);

        var token =
            Convert.ToBase64String(tokenBytes);

        // 6. Hash token before storing it
        var tokenHash =
            Convert.ToHexString(
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(token)));

        // 7. Create database entity
        var resetToken = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = tokenHash,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt =
                DateTime.UtcNow.AddMinutes(30)
        };

        await _passwordResetTokenRepository
            .CreateAsync(resetToken);

        // 8. Create reset link
        var resetLink =
     $"http://localhost:4200/auth/reset-password?token={Uri.EscapeDataString(token)}";

        // 9. Send email
        await _emailService.SendPasswordResetEmailAsync(
            user.Email,
            user.Name,
            resetLink);
    }

    public async Task ResetPasswordAsync(
       ResetPasswordRequest request)
    {
        // 1. Validate request
        await _resetPasswordValidator
            .ValidateAndThrowAsync(request);

        // 2. Decode URL-encoded token if necessary
        var token = Uri.UnescapeDataString(request.Token);

        // 3. Hash the original token
        var tokenHash = Convert.ToHexString(
            SHA256.HashData(
                Encoding.UTF8.GetBytes(token)));

        // 4. Find token
        var resetToken =
            await _passwordResetTokenRepository
                .GetByTokenHashAsync(tokenHash);

        if (resetToken == null)
        {
            throw new InvalidOperationException(
                "Invalid or expired password reset token.");
        }

        // 5. Check whether token was already used
        if (resetToken.UsedAt.HasValue)
        {
            throw new InvalidOperationException(
                "This password reset token has already been used.");
        }

        // 6. Check expiration
        if (resetToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "This password reset token has expired.");
        }

        // 7. Make sure user exists
        var user =
            await _userRepository.GetByIdAsync(
                resetToken.UserId);

        if (user == null)
        {
            throw new InvalidOperationException(
                "User not found.");
        }

        // 8. Hash new password
        user.PasswordHash =
            _passwordHasher.Hash(
                request.NewPassword);

        // 9. Update password
        await _userRepository.UpdateAsync(user);

        // 10. Mark reset token as used
        await _passwordResetTokenRepository
            .MarkAsUsedAsync(resetToken);

        // 11. Revoke all existing refresh tokens
        await _refreshTokenRepository
            .RevokeAllByUserIdAsync(user.Id);
    }
}