using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _context;

    public CartRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Carts
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                    .ThenInclude(x => x.Images)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<Cart?> GetByIdAsync(Guid cartId)
    {
        return await _context.Carts
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                    .ThenInclude(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == cartId);
    }

    public async Task<Cart> CreateAsync(Cart cart)
    {
        await _context.Carts.AddAsync(cart);
        await _context.SaveChangesAsync();

        return cart;
    }

    public async Task<CartItem?> GetItemAsync(
        Guid cartId,
        Guid productId)
    {
        return await _context.CartItems
            .FirstOrDefaultAsync(x =>
                x.CartId == cartId &&
                x.ProductId == productId);
    }

    public async Task<CartItem?> GetItemByIdAsync(
        Guid cartItemId)
    {
        return await _context.CartItems
            .Include(x => x.Cart)
            .Include(x => x.Product)
                .ThenInclude(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == cartItemId);
    }

    public async Task<CartItem> AddItemAsync(
        CartItem cartItem)
    {
        await _context.CartItems.AddAsync(cartItem);
        await _context.SaveChangesAsync();

        return cartItem;
    }

    public async Task<CartItem> UpdateItemAsync(
        CartItem cartItem)
    {
        _context.CartItems.Update(cartItem);
        await _context.SaveChangesAsync();

        return cartItem;
    }

    public async Task DeleteItemAsync(
        CartItem cartItem)
    {
        _context.CartItems.Remove(cartItem);

        await _context.SaveChangesAsync();
    }

    public async Task ClearAsync(Cart cart)
    {
        _context.CartItems.RemoveRange(cart.Items);

        await _context.SaveChangesAsync();
    }
}