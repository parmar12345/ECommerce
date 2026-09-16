using System.Security.Claims;
using ECommerce.Application.DTOs.Addresses;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressController(
        IAddressService addressService)
    {
        _addressService = addressService;
    }

    [HttpPost]
    public async Task<ActionResult<AddressResponse>> Create(
        [FromBody] CreateAddressRequest request)
    {
        var userId = GetCurrentUserId();

        var address =
            await _addressService.CreateAsync(
                userId,
                request);

        return Ok(address);
    }

    [HttpGet]
    public async Task<ActionResult<List<AddressResponse>>> GetAll()
    {
        var userId = GetCurrentUserId();

        var addresses =
            await _addressService.GetAllAsync(userId);

        return Ok(addresses);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AddressResponse>> GetById(
        Guid id)
    {
        var userId = GetCurrentUserId();

        var address =
            await _addressService.GetByIdAsync(
                userId,
                id);

        return Ok(address);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AddressResponse>> Update(
        Guid id,
        [FromBody] UpdateAddressRequest request)
    {
        var userId = GetCurrentUserId();

        var address =
            await _addressService.UpdateAsync(
                userId,
                id,
                request);

        return Ok(address);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        var userId = GetCurrentUserId();

        await _addressService.DeleteAsync(
            userId,
            id);

        return NoContent();
    }

    [HttpPut("{id:guid}/default")]
    public async Task<IActionResult> SetDefault(
        Guid id)
    {
        var userId = GetCurrentUserId();

        await _addressService.SetDefaultAsync(
            userId,
            id);

        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(
                userIdClaim,
                out var userId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user identity.");
        }

        return userId;
    }
}