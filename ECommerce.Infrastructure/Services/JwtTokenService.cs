using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECommerce.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ECommerce.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(
        Guid userId,
        string email,
        string name)
    {
        var secretKey =
            _configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException(
                "JWT SecretKey is not configured.");

        var issuer =
            _configuration["JwtSettings:Issuer"];

        var audience =
            _configuration["JwtSettings:Audience"];

        var expirationMinutes =
            int.Parse(
                _configuration["JwtSettings:ExpirationMinutes"]
                ?? "30");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                userId.ToString()),

            new Claim(
                ClaimTypes.Email,
                email),

            new Claim(
                ClaimTypes.Name,
                name)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                expirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    public DateTime GetExpirationTime()
    {
        var expirationMinutes =
            int.Parse(
                _configuration["JwtSettings:ExpirationMinutes"]
                ?? "30");

        return DateTime.UtcNow.AddMinutes(
            expirationMinutes);
    }
}