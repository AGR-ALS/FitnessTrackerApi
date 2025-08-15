namespace FitnessTracker.Domain.Models;

public class RefreshToken
{
    private RefreshToken(string id, string token, string userId, DateTime expiresAt)
    {
        Id = id;
        Token = token;
        UserId = userId;
        ExpiresAt = expiresAt;
    }

    public static RefreshToken Create(string token, string userId, DateTime expiresAt)
    {
        return new RefreshToken(Guid.NewGuid().ToString(), token, userId, expiresAt);
    }

    public string Id { get;  }
    public string Token { get;  }
    public string UserId { get;  }
    public DateTime ExpiresAt { get;  }
}