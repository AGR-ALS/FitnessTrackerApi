namespace FitnessTracker.Business.Abstractions.Authentication;

public interface ICurrentUserDataService
{
    string? Id { get; }
    string? Email { get; }
    string? Username { get; }
}