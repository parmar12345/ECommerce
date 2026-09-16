using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(string email);

    Task<User> CreateAsync(User user);

    Task<User?> GetByIdAsync(Guid userId);

    Task<User?> GetByEmailAsync(string email);

    Task MarkEmailAsVerifiedAsync(Guid userId);

    Task UpdateAsync(User user);
}