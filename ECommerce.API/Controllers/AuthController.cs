using ECommerce.Application.DTOs.Auth;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEmailVerificationService _emailVerificationService;

    public AuthController(
        IAuthService authService,
        IEmailVerificationService emailVerificationService)
    {
        _authService = authService;
        _emailVerificationService = emailVerificationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
     [FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);

        return StatusCode(
            StatusCodes.Status201Created,
            result);
    }

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail(
    [FromQuery] string token)
    {
        await _emailVerificationService.VerifyEmailAsync(token);

        return Ok(new
        {
            message = "Email verified successfully."
        });
    }

    [HttpPost("resend-verification")]
    [EnableRateLimiting("resend-verification")]
    public async Task<IActionResult> ResendVerification(
    ResendVerificationRequest request)
    {
        await _emailVerificationService
            .ResendVerificationEmailAsync(request.Email);

        return Ok(new
        {
            message = "If the email requires verification, a new verification email has been sent."
        });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(
    LoginRequest request)
    {
        var result =
            await _authService.LoginAsync(request);

        Response.Cookies.Append(
     "refreshToken",
     result.RefreshToken,
     new CookieOptions
     {
         HttpOnly = true,
         Secure = true,
         SameSite = SameSiteMode.None,
         Expires =
             DateTimeOffset.UtcNow.AddDays(7)
     });

        return Ok(result.Response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken =
            Request.Cookies["refreshToken"];

        var result =
            await _authService.RefreshTokenAsync(
                refreshToken ?? string.Empty);

        Response.Cookies.Append(
            "refreshToken",
            result.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires =
                    DateTimeOffset.UtcNow.AddDays(7)
            });

        return Ok(result.Response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshToken =
            Request.Cookies["refreshToken"];

        await _authService.LogoutAsync(
            refreshToken ?? string.Empty);

        Response.Cookies.Delete(
            "refreshToken",
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

        return Ok(new
        {
            message = "Logout successful."
        });
    }

    [Authorize]
    [HttpGet("get-me")]
    public async Task<IActionResult> GetMe()
    {
        var userIdClaim =
            User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            throw new UnauthorizedAccessException(
                "User identity could not be determined.");
        }

        if (!Guid.TryParse(
            userIdClaim.Value,
            out var userId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user identity.");
        }

        var response =
            await _authService.GetMeAsync(userId);

        return Ok(response);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(
    ForgotPasswordRequest request)
    {
        await _authService.ForgotPasswordAsync(
            request);

        return Ok(new
        {
            message =
                "If an account exists with this email, " +
                "a password reset link has been sent."
        });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
    ResetPasswordRequest request)
    {
        await _authService.ResetPasswordAsync(
            request);

        return Ok(new
        {
            message = "Password has been reset successfully."
        });
    }
}