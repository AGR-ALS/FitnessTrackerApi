using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FitnessTracker.Domain.Models;
using FitnessTracker.Infrastructure.Authentication.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FitnessTacker.Tests.InfrastructureTests.Jwt
{
    public class JwtTokenGeneratorTests
    {
        private JwtTokenGenerator CreateGenerator(
            string secret = "abcdefghijklmnopqrstuvwxyz123456", 
            string issuer = "TestIssuer",
            string audience = "TestAudience",
            int expiryHours = 2)
        {
            var settings = new JwtSettings
            {
                SecretKey = secret,
                Issuer = issuer,
                Audience = audience,
                ExpirationMinutesForAccessToken = expiryHours
            };
            return new JwtTokenGenerator(Options.Create(settings));
        }

        [Fact]
        public void GenerateJwtToken_IncludesAllClaimsAndCorrectProperties()
        {
            var userId = Guid.NewGuid().ToString();
            var userName = "testuser";
            var userEmail = "test@example.com";
            var passwordHash = "irrelevantHash";
            var user = User.RestoreFromEntity(userId, userName, userEmail, passwordHash);

            var secret = "abcdefghijklmnopqrstuvwxyz123456";
            var issuer = "MyApp";
            var audience = "MyUsers";
            var expiryMinutes = 5;
            var generator = CreateGenerator(secret, issuer, audience, expiryMinutes);

            var tokenString = generator.GenerateJwtToken(user);

            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(tokenString));

            var token = handler.ReadJwtToken(tokenString);

            Assert.Equal(issuer, token.Issuer);
            Assert.Contains(audience, token.Audiences);

            var now = DateTime.UtcNow;
            Assert.True(token.ValidTo > now.AddMinutes(expiryMinutes - 0.1));
            Assert.True(token.ValidTo <= now.AddMinutes(expiryMinutes).AddMinutes(1));

            var claims = token.Claims.ToDictionary(c => c.Type, c => c.Value);
            Assert.Equal(userId, claims["Id"]);
            Assert.Equal(userEmail, claims["Email"]);
            Assert.Equal(userName, claims["Username"]);
        }

        [Fact]
        public void GenerateJwtToken_SignedWithCorrectKey()
        {
            var user = User.RestoreFromEntity("u1", "name1", "e1@example.com", "hash");
            var secret = "secretkeysecretkeysecretkeysecretkeysecretkeysecretkeysecretkeysecretkeysecretkeysecretkeysecretkeysecretkey"; 
            var generator = CreateGenerator(secret, "iss", "aud", 1);

            var tokenString = generator.GenerateJwtToken(user);

            var handler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "iss",
                ValidateAudience = true,
                ValidAudience = "aud",
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
            };

            handler.ValidateToken(tokenString, validationParameters, out var validated);
            Assert.IsType<JwtSecurityToken>(validated);
        }
    }
}
