namespace FitnessTracker.Domain.Models;

public interface IDocument
{
    string Id { get; }
    DateTime CreatedAt { get; }
}