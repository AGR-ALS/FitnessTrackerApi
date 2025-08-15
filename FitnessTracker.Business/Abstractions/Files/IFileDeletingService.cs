namespace FitnessTracker.Business.Abstractions.Files;

public interface IFileDeletingService
{ 
    Task DeleteFileAsync(List<string> filesToDelete);
}