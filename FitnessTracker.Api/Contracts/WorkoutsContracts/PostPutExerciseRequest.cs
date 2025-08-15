using System.ComponentModel.DataAnnotations;

namespace FitnessTrackerApi.Contracts.WorkoutsContracts;

public record PostPutExerciseRequest
{
    
    
    public required string Name { get; init; }
    
    
    
    public List<PostPutSetRequest> Sets { get; init; }
}