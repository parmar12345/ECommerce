using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IAddressRepository
{
    Task<List<Address>> GetByUserIdAsync(Guid userId);

    Task<Address?> GetByIdAsync(Guid addressId);

    Task<Address?> GetByIdForUserAsync(
        Guid addressId,
        Guid userId);

    Task<Address> CreateAsync(Address address);

    Task<Address> UpdateAsync(Address address);

    Task DeleteAsync(Address address);

    Task UnsetDefaultAddressesAsync(Guid userId);
}