using ECommerce.Application.DTOs.Addresses;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;
using FluentValidation;

namespace ECommerce.Infrastructure.Services;

public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;

    private readonly IValidator<CreateAddressRequest>
        _createValidator;

    private readonly IValidator<UpdateAddressRequest>
        _updateValidator;

    public AddressService(
        IAddressRepository addressRepository,
        IValidator<CreateAddressRequest> createValidator,
        IValidator<UpdateAddressRequest> updateValidator)
    {
        _addressRepository = addressRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<AddressResponse> CreateAsync(
        Guid userId,
        CreateAddressRequest request)
    {
        await _createValidator.ValidateAndThrowAsync(request);

        var existingAddresses =
            await _addressRepository.GetByUserIdAsync(userId);

        var address = new Address
        {
            Id = Guid.NewGuid(),

            UserId = userId,

            FullName = request.FullName.Trim(),

            PhoneNumber = request.PhoneNumber.Trim(),

            AddressLine1 = request.AddressLine1.Trim(),

            AddressLine2 =
                string.IsNullOrWhiteSpace(request.AddressLine2)
                    ? null
                    : request.AddressLine2.Trim(),

            City = request.City.Trim(),

            State = request.State.Trim(),

            PostalCode = request.PostalCode.Trim(),

            Country = request.Country.Trim(),

            IsDefault = existingAddresses.Count == 0,

            CreatedAt = DateTime.UtcNow
        };

        await _addressRepository.CreateAsync(address);

        return MapToResponse(address);
    }

    public async Task<List<AddressResponse>> GetAllAsync(
        Guid userId)
    {
        var addresses =
            await _addressRepository.GetByUserIdAsync(userId);

        return addresses
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<AddressResponse> GetByIdAsync(
        Guid userId,
        Guid addressId)
    {
        var address =
            await _addressRepository.GetByIdForUserAsync(
                addressId,
                userId);

        if (address is null)
        {
            throw new KeyNotFoundException(
                "Address not found.");
        }

        return MapToResponse(address);
    }

    public async Task<AddressResponse> UpdateAsync(
        Guid userId,
        Guid addressId,
        UpdateAddressRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);

        var address =
            await _addressRepository.GetByIdForUserAsync(
                addressId,
                userId);

        if (address is null)
        {
            throw new KeyNotFoundException(
                "Address not found.");
        }

        address.FullName =
            request.FullName.Trim();

        address.PhoneNumber =
            request.PhoneNumber.Trim();

        address.AddressLine1 =
            request.AddressLine1.Trim();

        address.AddressLine2 =
            string.IsNullOrWhiteSpace(request.AddressLine2)
                ? null
                : request.AddressLine2.Trim();

        address.City =
            request.City.Trim();

        address.State =
            request.State.Trim();

        address.PostalCode =
            request.PostalCode.Trim();

        address.Country =
            request.Country.Trim();

        address.UpdatedAt =
            DateTime.UtcNow;

        await _addressRepository.UpdateAsync(address);

        return MapToResponse(address);
    }

    public async Task DeleteAsync(
        Guid userId,
        Guid addressId)
    {
        var address =
            await _addressRepository.GetByIdForUserAsync(
                addressId,
                userId);

        if (address is null)
        {
            throw new KeyNotFoundException(
                "Address not found.");
        }

        await _addressRepository.DeleteAsync(address);

        if (address.IsDefault)
        {
            var remainingAddresses =
                await _addressRepository.GetByUserIdAsync(userId);

            var newDefault =
                remainingAddresses.FirstOrDefault();

            if (newDefault is not null)
            {
                newDefault.IsDefault = true;
                newDefault.UpdatedAt = DateTime.UtcNow;

                await _addressRepository.UpdateAsync(
                    newDefault);
            }
        }
    }

    public async Task SetDefaultAsync(
        Guid userId,
        Guid addressId)
    {
        var address =
            await _addressRepository.GetByIdForUserAsync(
                addressId,
                userId);

        if (address is null)
        {
            throw new KeyNotFoundException(
                "Address not found.");
        }

        if (address.IsDefault)
        {
            return;
        }

        await _addressRepository
            .UnsetDefaultAddressesAsync(userId);

        address.IsDefault = true;
        address.UpdatedAt = DateTime.UtcNow;

        await _addressRepository.UpdateAsync(address);
    }

    private static AddressResponse MapToResponse(
        Address address)
    {
        return new AddressResponse
        {
            Id = address.Id,

            FullName = address.FullName,

            PhoneNumber = address.PhoneNumber,

            AddressLine1 = address.AddressLine1,

            AddressLine2 = address.AddressLine2,

            City = address.City,

            State = address.State,

            PostalCode = address.PostalCode,

            Country = address.Country,

            IsDefault = address.IsDefault,

            CreatedAt = address.CreatedAt,

            UpdatedAt = address.UpdatedAt
        };
    }
}