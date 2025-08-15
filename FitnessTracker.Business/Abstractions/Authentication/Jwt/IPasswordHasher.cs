namespace FitnessTracker.Business.Abstractions.Authentication.Jwt;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyHashedPassword(string providedPassword, string hashedPassword);
}