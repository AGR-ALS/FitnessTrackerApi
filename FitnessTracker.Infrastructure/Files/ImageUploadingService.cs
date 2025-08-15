using FitnessTracker.Business.Abstractions.Files;
using FitnessTracker.Business.Abstractions.Files.ImageUploading;

namespace FitnessTracker.Infrastructure.Files;

public class ImageUploadingService : IImageUploadingService
{
    private readonly IImageUploader _imageUploader;

    public ImageUploadingService(IImageUploader imageUploader)
    {
        _imageUploader = imageUploader;
    }
    
    public async Task<List<string>> UploadImageAsync(IEnumerable<IFormFileAdapter>? files, string dirToUpload = "uploads")
    {
        var uploadedFiles = new List<string>();
        if (files == null)
        {
            return uploadedFiles;
        }
        foreach (var file in files)
        {
            var uploadedFile = await _imageUploader.UploadImageAsync(file, dirToUpload);
            if (uploadedFile != null)
            {
                uploadedFiles.Add(uploadedFile);
            }
        }
        return uploadedFiles;
    }
}