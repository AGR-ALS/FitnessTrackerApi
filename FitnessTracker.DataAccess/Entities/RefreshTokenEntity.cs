using FitnessTracker.Domain.Models;

namespace FitnessTracker.DataAccess.Entities;

public class RefreshTokenEntity
{
    public required string Id { get; set; }
    public required string Token { get; set; }
    public required string UserId { get; set; }
    public UserEntity User { get; set; }
    public DateTime ExpiresAt { get; set; }
}