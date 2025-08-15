using FitnessTracker.Business.Abstractions.Files;
using FitnessTracker.Business.Exceptions;

namespace FitnessTracker.Infrastructure.Files;

public class FileDeletingService : IFileDeletingService
{
    public async Task DeleteFileAsync(List<string> filesToDelete)
    {
        await Task.Run(() =>
        {
            foreach (var fileToDelete in filesToDelete)
            {
                if (!File.Exists(fileToDelete))
                {
                    throw new NotFoundException("Files was not found");
                }
                File.Delete(fileToDelete);
            }
        });
    }
}