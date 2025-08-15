using FitnessTracker.Domain.Models;

namespace FitnessTracker.Business.Abstractions.Authentication;

public interface IRefreshTokenGenerator
{
    string GenerateRefreshToken();
    bool VerifyRefreshToken(RefreshToken refreshToken);
}