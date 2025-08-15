namespace FitnessTracker.Infrastructure.Authentication.Jwt;

public class JwtSettings
{
    public string SecretKey { get; set; }
    public int ExpirationMinutesForAccessToken { get; set; }
    public int ExpirationDaysForRefreshToken { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}