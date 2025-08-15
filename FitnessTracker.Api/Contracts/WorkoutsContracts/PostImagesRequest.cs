using System.ComponentModel.DataAnnotations;

namespace FitnessTrackerApi.Contracts.WorkoutsContracts;

public record PostImagesRequest
{
    
    public required List<IFormFile> ProgressPhotos { get; init; }
    
}