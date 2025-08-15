namespace FitnessTracker.Domain.Models;

public class User
{
    private User(string id, string name, string email, string passwordHash)
    {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
    }
    
    public string Id { get; }
    public string Name { get; }
    public string Email { get; }
    public string PasswordHash { get; }

    public static User Create(string name, string email, string passwordHash)
    {
        return new User(Guid.NewGuid().ToString(), name, email, passwordHash);
    }

    public static User RestoreFromEntity(string id, string name, string email, string passwordHash)
    {
        return new User(id, name, email, passwordHash);
    }
}