using FitnessTracker.Business.Abstractions.Authentication.Jwt;
using FitnessTracker.Infrastructure.Authentication.Jwt;

namespace FitnessTacker.Tests.InfrastructureTests.Jwt;

public class PasswordHasherTests
{
    private readonly IPasswordHasher _hasher = new PasswordHasher();
    
    [Theory]
    [InlineData("Password1!")]
    [InlineData("Another$ecret")]
    public void VerifyHashedPassword_ReturnsTrue_ForCorrectPassword(string password)
    {
        var hash = _hasher.HashPassword(password);

        var result = _hasher.VerifyHashedPassword(password, hash);

        Assert.True(result);
    }

    [Theory]
    [InlineData("Password1!", "WrongPassword")]
    [InlineData("abc123", "ABC123")]
    public void VerifyHashedPassword_ReturnsFalse_ForIncorrectPassword(string correct, string wrong)
    {
        var hash = _hasher.HashPassword(correct);

        var result = _hasher.VerifyHashedPassword(wrong, hash);

        Assert.False(result);
    }
    
}