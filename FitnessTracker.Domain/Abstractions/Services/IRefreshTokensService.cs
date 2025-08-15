using FitnessTracker.Domain.Models;

namespace FitnessTracker.Domain.Abstractions.Services;

public interface IRefreshTokensService
{
    Task<string> AddRefreshToken(string userId, CancellationToken cancellationToken);
    Task DeleteRefreshToken(string token, CancellationToken cancellationToken);
    Task<bool> ValidateRefreshToken(string token, CancellationToken cancellationToken);
    Task<RefreshToken> GetRefreshTokenModel(string token, CancellationToken cancellationToken);
}