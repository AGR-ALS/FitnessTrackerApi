namespace FitnessTrackerApi.Contracts.WorkoutsContracts;

public class PutImagesRequest
{
    public List<IFormFile>? ProgressPhotos { get; init; }
}