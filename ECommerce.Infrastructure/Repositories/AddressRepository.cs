using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly ApplicationDbContext _context;

    public AddressRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Address>> GetByUserIdAsync(
        Guid userId)
    {
        return await _context.Addresses
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.IsDefault)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Address?> GetByIdAsync(
        Guid addressId)
    {
        return await _context.Addresses
            .FirstOrDefaultAsync(
                x => x.Id == addressId);
    }

    public async Task<Address?> GetByIdForUserAsync(
        Guid addressId,
        Guid userId)
    {
        return await _context.Addresses
            .FirstOrDefaultAsync(
                x =>
                    x.Id == addressId &&
                    x.UserId == userId);
    }

    public async Task<Address> CreateAsync(
        Address address)
    {
        await _context.Addresses.AddAsync(address);

        await _context.SaveChangesAsync();

        return address;
    }

    public async Task<Address> UpdateAsync(
        Address address)
    {
        _context.Addresses.Update(address);

        await _context.SaveChangesAsync();

        return address;
    }

    public async Task DeleteAsync(
        Address address)
    {
        _context.Addresses.Remove(address);

        await _context.SaveChangesAsync();
    }

    public async Task UnsetDefaultAddressesAsync(
        Guid userId)
    {
        await _context.Addresses
            .Where(x =>
                x.UserId == userId &&
                x.IsDefault)
            .ExecuteUpdateAsync(
                setters =>
                    setters.SetProperty(
                        x => x.IsDefault,
                        false));
    }
}