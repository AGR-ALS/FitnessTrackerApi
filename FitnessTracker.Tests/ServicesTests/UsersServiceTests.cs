using System.Security.Authentication;
using FitnessTracker.Business.Abstractions.Authentication;
using FitnessTracker.Business.Abstractions.Authentication.Jwt;
using FitnessTracker.Business.Services;
using FitnessTracker.Domain.Abstractions.Repositories;
using FitnessTracker.Domain.Abstractions.Services;
using FitnessTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FitnessTacker.Tests.ServicesTests
{
    public class UsersServiceTests
    {
        private readonly Mock<IUsersRepository> _usersRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
        private readonly Mock<IRefreshTokensService> _refreshTokensServiceMock;
        private readonly UsersService _service;

        public UsersServiceTests()
        {
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
            _refreshTokensServiceMock = new Mock<IRefreshTokensService>();

            _service = new UsersService(
                _usersRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtTokenGeneratorMock.Object,
            _refreshTokensServiceMock.Object
                );
        }

        [Fact]
        public async Task RegisterUser_CallsAddUserAsync_WithHashedPassword()
        {
            var username = "testuser";
            var email = "test@example.com";
            var password = "password123";
            var hashed = "hashedpwd";

            _passwordHasherMock
                .Setup(h => h.HashPassword(password))
                .Returns(hashed);

            await _service.RegisterUser(username, email, password, CancellationToken.None);

            _usersRepositoryMock.Verify(r => r.AddUserAsync(
                It.Is<User>(u => u.Name == username && u.Email == email && u.PasswordHash == hashed),
                CancellationToken.None
            ), Times.Once);
        }

        [Fact]
        public async Task RegisterUser_OnDbUpdateException_ThrowsInvalidOperationException()
        {
            var ex = new DbUpdateException("db error", new Exception("inner msg"));
            _usersRepositoryMock
                .Setup(r => r.AddUserAsync(It.IsAny<User>(), CancellationToken.None))
                .ThrowsAsync(ex);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.RegisterUser("u", "e", "p", CancellationToken.None)
            );
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            var username = "testName";
            var email = "test@example.com";
            var password = "password123";
            var passwordHash = "hashedpwd";
            var token = "jwt-token";
            var user = User.Create(username, email, passwordHash);

            _usersRepositoryMock
                .Setup(r => r.GetUserByEmailAsync(email, CancellationToken.None))
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.VerifyHashedPassword(password, passwordHash))
                .Returns(true);

            _jwtTokenGeneratorMock
                .Setup(j => j.GenerateJwtToken(user))
                .Returns(token);

            var (result, _) = await _service.Login(email, password, CancellationToken.None);

            Assert.Equal(token, result);
        }

        [Theory]
        [InlineData(null, "password")]
        [InlineData("test@example.com", "wrongpass")]
        public async Task Login_WithInvalidCredentials_ThrowsInvalidCredentialException(string? email, string password)
        {
            User? user = null;
            _usersRepositoryMock
                .Setup(r => r.GetUserByEmailAsync(It.IsAny<string>(), CancellationToken.None))
                .ReturnsAsync((User?)null);

            if (email != null)
            {
                user = User.Create("testName", email, password);
                _usersRepositoryMock
                    .Setup(r => r.GetUserByEmailAsync(email, CancellationToken.None))
                    .ReturnsAsync(user);

                _passwordHasherMock
                    .Setup(h => h.VerifyHashedPassword(password, user.PasswordHash))
                    .Returns(false);
            }

            await Assert.ThrowsAsync<InvalidCredentialException>(
                () => _service.Login(email, password, CancellationToken.None)
            );
        }
    }
}