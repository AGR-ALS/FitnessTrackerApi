using System.Security.Cryptography;
using FitnessTracker.Business.Abstractions.Authentication;
using FitnessTracker.Domain.Models;

namespace FitnessTracker.Infrastructure.Authentication;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }

    public bool VerifyRefreshToken(RefreshToken refreshToken)
    {
        return refreshToken.ExpiresAt >= DateTime.UtcNow;
    }
}