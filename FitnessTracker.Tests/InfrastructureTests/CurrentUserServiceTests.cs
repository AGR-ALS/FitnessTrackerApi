using System.Security.Claims;
using FitnessTracker.Infrastructure;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FitnessTacker.Tests.InfrastructureTests;

public class CurrentUserServiceTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly CurrentUserDataService _service;

    public CurrentUserServiceTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _service = new CurrentUserDataService(_httpContextAccessorMock.Object);
    }
    
    [Fact]
    public void Username_ReturnsValue_WhenClaimPresent()
    {
        var claims = new[]
        {
            new Claim("Id", "user-123"),
            new Claim("Email", "test@example.com"),
            new Claim("Username", "testuser")
        };
        var identity = new ClaimsIdentity(claims);
        var user = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = user };
        _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);

        var idResult = _service.Id;
        var emailResult = _service.Email;
        var usernameResult = _service.Username;

        Assert.Equal("user-123", idResult);
        Assert.Equal("test@example.com", emailResult);
        Assert.Equal("testuser", usernameResult);
    }

}