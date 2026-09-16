using ECommerce.Application.DTOs.Cart;

namespace ECommerce.Application.Interfaces.Services;

public interface ICartService
{
    Task<CartResponse> GetCartAsync(Guid userId);

    Task<CartResponse> AddItemAsync(
        Guid userId,
        AddToCartRequest request);

    Task<CartResponse> UpdateItemAsync(
        Guid userId,
        Guid cartItemId,
        UpdateCartItemRequest request);

    Task RemoveItemAsync(
        Guid userId,
        Guid cartItemId);

    Task ClearCartAsync(Guid userId);
}