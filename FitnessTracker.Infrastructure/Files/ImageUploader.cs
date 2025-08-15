using FitnessTracker.Business.Abstractions.Files;
using FitnessTracker.Business.Abstractions.Files.ImageUploading;
using Microsoft.AspNetCore.Hosting;

namespace FitnessTracker.Infrastructure.Files;

public class ImageUploader : IImageUploader
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ImageUploader(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }
    
    public async Task<string?> UploadImageAsync(IFormFileAdapter? file, string dirToUpload = "uploads")
    {
        string? filePath = null;
        if (file != null)
        {
            try
            {
                var uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, dirToUpload);
                Directory.CreateDirectory(uploadsDir);
            
                var fileName =  Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                filePath = Path.Combine(uploadsDir, fileName);
            }
            catch (NullReferenceException ex)
            {
                throw new NullReferenceException("No web root folder was configured(such as wwwroot)", ex);
            }
            using (var stream =  new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
        return filePath;
    }
}