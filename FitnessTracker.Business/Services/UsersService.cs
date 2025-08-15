using System.Security.Authentication;
using FitnessTracker.Business.Abstractions.Authentication;
using FitnessTracker.Business.Abstractions.Authentication.Jwt;
using FitnessTracker.Domain.Abstractions.Repositories;
using FitnessTracker.Domain.Abstractions.Services;
using FitnessTracker.Domain.Models;

namespace FitnessTracker.Business.Services;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRefreshTokensService _refreshTokensService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public UsersService(IUsersRepository usersRepository, IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator, IRefreshTokensService refreshTokensService)
    {
        _usersRepository = usersRepository;
        _passwordHasher = passwordHasher;
        _refreshTokensService = refreshTokensService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task RegisterUser(string username, string email, string password, CancellationToken cancellationToken)
    {
        var hashedPassword = _passwordHasher.HashPassword(password);
        try
        {
            await _usersRepository.AddUserAsync(User.Create(username, email, hashedPassword), cancellationToken);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message);
        }
    }

    public async Task<(string, string)> Login(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetUserByEmailAsync(email, cancellationToken);
        var loginResult = user != null && _passwordHasher.VerifyHashedPassword(password, user.PasswordHash);
        if (!loginResult)
        {
            throw new InvalidCredentialException("Invalid login or password");
        }
        var accessToken = _jwtTokenGenerator.GenerateJwtToken(user!);
        var refreshToken = await _refreshTokensService.AddRefreshToken(user!.Id, cancellationToken);
        return (accessToken, refreshToken);
    }

    public async Task<string> Login(string passedRefreshToken, CancellationToken cancellationToken)
    {
        if (!(await _refreshTokensService.ValidateRefreshToken(passedRefreshToken, cancellationToken)))
        {
            await _refreshTokensService.DeleteRefreshToken(passedRefreshToken, cancellationToken);
            throw new UnauthorizedAccessException();
        }
        var refreshToken = await _refreshTokensService.GetRefreshTokenModel(passedRefreshToken, cancellationToken);
        var user = await _usersRepository.GetUserByIdAsync(refreshToken.UserId, cancellationToken);
        var accessToken = _jwtTokenGenerator.GenerateJwtToken(user!);
        return accessToken;
    }
}