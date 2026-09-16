using System.Security.Claims;
using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<ActionResult<CartResponse>> GetCart()
    {
        var userId = GetCurrentUserId();

        var cart = await _cartService.GetCartAsync(userId);

        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartResponse>> AddItem(
        [FromBody] AddToCartRequest request)
    {
        var userId = GetCurrentUserId();

        var cart = await _cartService.AddItemAsync(
            userId,
            request);

        return Ok(cart);
    }

    [HttpPut("items/{cartItemId:guid}")]
    public async Task<ActionResult<CartResponse>> UpdateItem(
        Guid cartItemId,
        [FromBody] UpdateCartItemRequest request)
    {
        var userId = GetCurrentUserId();

        var cart = await _cartService.UpdateItemAsync(
            userId,
            cartItemId,
            request);

        return Ok(cart);
    }

    [HttpDelete("items/{cartItemId:guid}")]
    public async Task<IActionResult> RemoveItem(
        Guid cartItemId)
    {
        var userId = GetCurrentUserId();

        await _cartService.RemoveItemAsync(
            userId,
            cartItemId);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var userId = GetCurrentUserId();

        await _cartService.ClearCartAsync(userId);

        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user identity.");
        }

        return userId;
    }
}