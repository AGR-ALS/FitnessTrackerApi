namespace FitnessTracker.Domain.Abstractions.Services;

public interface IUsersService
{
    Task RegisterUser(string username, string email, string password, CancellationToken cancellationToken);
    Task<(string, string)> Login(string email, string password, CancellationToken cancellationToken);
    Task<string> Login(string refreshToken, CancellationToken cancellationToken);
}