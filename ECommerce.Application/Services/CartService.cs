using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartResponse> GetCartAsync(Guid userId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart is null)
        {
            return new CartResponse
            {
                Id = Guid.Empty,
                Items = new List<CartItemResponse>(),
                TotalItems = 0,
                Subtotal = 0
            };
        }

        return MapToResponse(cart);
    }

    public async Task<CartResponse> AddItemAsync(
        Guid userId,
        AddToCartRequest request)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");
        }

        var product = await _productRepository
            .GetByIdAsync(request.ProductId);

        if (product is null)
        {
            throw new InvalidOperationException(
                "Product not found.");
        }

        if (request.Quantity > product.StockQuantity)
        {
            throw new InvalidOperationException(
                "Requested quantity exceeds available stock.");
        }

        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart is null)
        {
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _cartRepository.CreateAsync(cart);
        }

        var existingItem = await _cartRepository.GetItemAsync(
            cart.Id,
            product.Id);

        if (existingItem is not null)
        {
            var newQuantity =
                existingItem.Quantity + request.Quantity;

            if (newQuantity > product.StockQuantity)
            {
                throw new InvalidOperationException(
                    "Requested quantity exceeds available stock.");
            }

            existingItem.Quantity = newQuantity;
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.UpdateItemAsync(existingItem);
        }
        else
        {
            var cartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = request.Quantity
            };

            cart.UpdatedAt = DateTime.UtcNow;

            await _cartRepository.AddItemAsync(cartItem);
        }

        return await GetCartAsync(userId);
    }

    public async Task<CartResponse> UpdateItemAsync(
        Guid userId,
        Guid cartItemId,
        UpdateCartItemRequest request)
    {
        if (request.Quantity <= 0)
        {
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");
        }

        var cartItem = await _cartRepository
            .GetItemByIdAsync(cartItemId);

        if (cartItem is null)
        {
            throw new InvalidOperationException(
                "Cart item not found.");
        }

        if (cartItem.Cart.UserId != userId)
        {
            throw new InvalidOperationException(
                "Cart item does not belong to the current user.");
        }

        if (request.Quantity > cartItem.Product.StockQuantity)
        {
            throw new InvalidOperationException(
                "Requested quantity exceeds available stock.");
        }

        cartItem.Quantity = request.Quantity;
        cartItem.Cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.UpdateItemAsync(cartItem);

        return await GetCartAsync(userId);
    }

    public async Task RemoveItemAsync(
        Guid userId,
        Guid cartItemId)
    {
        var cartItem = await _cartRepository
            .GetItemByIdAsync(cartItemId);

        if (cartItem is null)
        {
            throw new InvalidOperationException(
                "Cart item not found.");
        }

        if (cartItem.Cart.UserId != userId)
        {
            throw new InvalidOperationException(
                "Cart item does not belong to the current user.");
        }

        cartItem.Cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.DeleteItemAsync(cartItem);
    }

    public async Task ClearCartAsync(Guid userId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);

        if (cart is null)
        {
            return;
        }

        cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.ClearAsync(cart);
    }

    private static CartResponse MapToResponse(Cart cart)
    {
        var items = cart.Items
            .Select(item =>
            {
                var primaryImage =
                    item.Product.Images
                        .FirstOrDefault(x => x.IsPrimary)
                    ?? item.Product.Images
                        .OrderBy(x => x.DisplayOrder)
                        .FirstOrDefault();

                return new CartItemResponse
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    UnitPrice = item.Product.Price,
                    Quantity = item.Quantity,
                    LineTotal = item.Product.Price * item.Quantity,
                    AvailableStock = item.Product.StockQuantity,
                    PrimaryImageUrl = primaryImage?.ImageUrl
                };
            })
            .ToList();

        return new CartResponse
        {
            Id = cart.Id,
            Items = items,
            TotalItems = items.Sum(x => x.Quantity),
            Subtotal = items.Sum(x => x.LineTotal)
        };
    }
}