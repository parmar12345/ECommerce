using ECommerce.Application.DTOs.Addresses;

namespace ECommerce.Application.Interfaces.Services;

public interface IAddressService
{
    Task<AddressResponse> CreateAsync(
        Guid userId,
        CreateAddressRequest request);

    Task<List<AddressResponse>> GetAllAsync(
        Guid userId);

    Task<AddressResponse> GetByIdAsync(
        Guid userId,
        Guid addressId);

    Task<AddressResponse> UpdateAsync(
        Guid userId,
        Guid addressId,
        UpdateAddressRequest request);

    Task DeleteAsync(
        Guid userId,
        Guid addressId);

    Task SetDefaultAsync(
        Guid userId,
        Guid addressId);
}