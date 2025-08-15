using FitnessTracker.Business.Abstractions.Authentication.Jwt;

namespace FitnessTracker.Infrastructure.Authentication.Jwt;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password);    
    }

    public bool VerifyHashedPassword(string providedPassword, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(providedPassword, hashedPassword);
    }
}