namespace FitnessTracker.Business.Abstractions.Files;

public interface IFormFileAdapter
{
    string FileName { get; }
    Task CopyToAsync(Stream target);
}