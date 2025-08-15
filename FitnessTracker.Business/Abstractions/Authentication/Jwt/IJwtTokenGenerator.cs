using FitnessTracker.Domain.Models;

namespace FitnessTracker.Business.Abstractions.Authentication.Jwt;

public interface IJwtTokenGenerator
{
    string GenerateJwtToken(User user);
}