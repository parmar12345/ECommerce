using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(Guid userId);

    Task<Cart?> GetByIdAsync(Guid cartId);

    Task<Cart> CreateAsync(Cart cart);

    Task<CartItem?> GetItemAsync(
        Guid cartId,
        Guid productId);

    Task<CartItem?> GetItemByIdAsync(
        Guid cartItemId);

    Task<CartItem> AddItemAsync(
        CartItem cartItem);

    Task<CartItem> UpdateItemAsync(
        CartItem cartItem);

    Task DeleteItemAsync(
        CartItem cartItem);

    Task ClearAsync(
        Cart cart);
}