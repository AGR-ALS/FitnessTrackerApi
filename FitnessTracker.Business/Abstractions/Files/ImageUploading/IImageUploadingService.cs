namespace FitnessTracker.Business.Abstractions.Files.ImageUploading;

public interface IImageUploadingService
{
    Task<List<string>> UploadImageAsync(IEnumerable<IFormFileAdapter>? files, string dirToUpload = "uploads");
}