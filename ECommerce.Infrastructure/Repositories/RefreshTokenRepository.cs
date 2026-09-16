using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class RefreshTokenRepository
    : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken> CreateAsync(
        RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(
            refreshToken);

        await _context.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash)
    {
        return await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash);
    }

    public async Task RevokeAsync(
        RefreshToken refreshToken)
    {
        refreshToken.RevokedAt =
            DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllByUserIdAsync(
    Guid userId)
    {
        var tokens =
            await _context.RefreshTokens
                .Where(x =>
                    x.UserId == userId &&
                    x.RevokedAt == null)
                .ToListAsync();

        if (tokens.Count == 0)
        {
            return;
        }

        var revokedAt = DateTime.UtcNow;

        foreach (var token in tokens)
        {
            token.RevokedAt = revokedAt;
        }

        await _context.SaveChangesAsync();
    }
}