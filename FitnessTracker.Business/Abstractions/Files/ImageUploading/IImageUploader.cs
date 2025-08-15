namespace FitnessTracker.Business.Abstractions.Files.ImageUploading;

public interface IImageUploader
{
    Task<string?> UploadImageAsync(IFormFileAdapter? file, string dirToUpload = "uploads");
}