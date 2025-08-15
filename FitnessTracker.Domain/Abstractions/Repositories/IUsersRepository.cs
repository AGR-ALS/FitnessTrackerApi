using FitnessTracker.Domain.Models;

namespace FitnessTracker.Domain.Abstractions.Repositories;

public interface IUsersRepository
{
    Task AddUserAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetUserByIdAsync(string id, CancellationToken cancellationToken);
}