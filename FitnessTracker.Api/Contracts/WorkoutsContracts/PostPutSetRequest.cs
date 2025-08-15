using System.ComponentModel.DataAnnotations;

namespace FitnessTrackerApi.Contracts.WorkoutsContracts;

public record PostPutSetRequest
{
    
    
    public int Reps { get; init; }
    
    
    public double Weight { get; init; }
}
    
