namespace FitnessTrackerApi.Contracts.UserContracts;

public record RegisterUserRequest(string Username, string Email, string Password);