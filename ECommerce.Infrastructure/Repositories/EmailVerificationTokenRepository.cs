using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class EmailVerificationTokenRepository
    : IEmailVerificationTokenRepository
{
    private readonly ApplicationDbContext _context;

    public EmailVerificationTokenRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmailVerificationToken> CreateAsync(
        EmailVerificationToken token)
    {
        await _context.EmailVerificationTokens.AddAsync(token);

        await _context.SaveChangesAsync();

        return token;
    }

    public async Task<EmailVerificationToken?> GetByTokenAsync(
        string token)
    {
        return await _context.EmailVerificationTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == token);
    }

    public async Task MarkAsUsedAsync(
        EmailVerificationToken token)
    {
        token.UsedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteByUserIdAsync(
        Guid userId)
    {
        var tokens = await _context.EmailVerificationTokens
            .Where(x => x.UserId == userId)
            .ToListAsync();

        if (tokens.Count == 0)
        {
            return;
        }

        _context.EmailVerificationTokens.RemoveRange(tokens);

        await _context.SaveChangesAsync();
    }


}