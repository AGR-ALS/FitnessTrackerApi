using FitnessTracker.Business.Abstractions.Authentication;
using FitnessTracker.Business.Exceptions;
using FitnessTracker.Domain.Abstractions.Repositories;
using FitnessTracker.Domain.Abstractions.Services;
using FitnessTracker.Domain.Models;
using FitnessTracker.Infrastructure.Authentication.Jwt;
using Microsoft.Extensions.Options;

namespace FitnessTracker.Infrastructure.Authentication;

public class RefreshTokensesService : IRefreshTokensService
{
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IRefreshTokensRepository _refreshTokensRepository;
    private readonly JwtSettings _jwtSettings;

    public RefreshTokensesService(IRefreshTokenGenerator refreshTokenGenerator, IRefreshTokensRepository refreshTokensRepository, ICurrentUserDataService currentUserDataService, IOptions<JwtSettings> jwtSettings)
    {
        _refreshTokenGenerator = refreshTokenGenerator;
        _refreshTokensRepository = refreshTokensRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<string> AddRefreshToken(string userId, CancellationToken cancellationToken)
    {
        var token = RefreshToken.Create(
            _refreshTokenGenerator.GenerateRefreshToken(),
            userId,
            DateTime.UtcNow.AddDays(_jwtSettings.ExpirationDaysForRefreshToken)
        );
        return await _refreshTokensRepository.AddRefreshToken(token, cancellationToken);
    }

    public async Task DeleteRefreshToken(string token, CancellationToken cancellationToken)
    {
        await _refreshTokensRepository.DeleteRefreshToken(token, cancellationToken);
    }

    public async Task<bool> ValidateRefreshToken(string token, CancellationToken cancellationToken)
    {
        RefreshToken refreshToken;
        try
        {
            refreshToken = await _refreshTokensRepository.GetRefreshToken(token, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("Refresh token was not found");
        }

        return _refreshTokenGenerator.VerifyRefreshToken(refreshToken);
    }

    public async Task<RefreshToken> GetRefreshTokenModel(string token, CancellationToken cancellationToken)
    {
        try
        {
            return await _refreshTokensRepository.GetRefreshToken(token, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("Refresh token was not found");
        }
    }
}