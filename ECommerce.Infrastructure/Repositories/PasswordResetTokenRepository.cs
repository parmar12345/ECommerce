using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class PasswordResetTokenRepository
    : IPasswordResetTokenRepository
{
    private readonly ApplicationDbContext _context;

    public PasswordResetTokenRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PasswordResetToken> CreateAsync(
        PasswordResetToken token)
    {
        await _context.PasswordResetTokens.AddAsync(token);

        await _context.SaveChangesAsync();

        return token;
    }

    public async Task<PasswordResetToken?> GetByTokenHashAsync(
        string tokenHash)
    {
        return await _context.PasswordResetTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.TokenHash == tokenHash);
    }

    public async Task DeleteByUserIdAsync(
        Guid userId)
    {
        var tokens =
            await _context.PasswordResetTokens
                .Where(x => x.UserId == userId)
                .ToListAsync();

        if (tokens.Count == 0)
            return;

        _context.PasswordResetTokens.RemoveRange(tokens);

        await _context.SaveChangesAsync();
    }

    public async Task MarkAsUsedAsync(
        PasswordResetToken token)
    {
        token.UsedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
}