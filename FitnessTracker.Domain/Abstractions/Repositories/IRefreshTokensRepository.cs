using FitnessTracker.Domain.Models;

namespace FitnessTracker.Domain.Abstractions.Repositories;

public interface IRefreshTokensRepository
{
    Task<string> AddRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task DeleteRefreshToken(string token, CancellationToken cancellationToken);
    Task<RefreshToken> GetRefreshToken(string token, CancellationToken cancellationToken);
}