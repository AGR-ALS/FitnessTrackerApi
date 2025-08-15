using System.ComponentModel.DataAnnotations;
using FitnessTracker.Domain.Models;

namespace FitnessTrackerApi.Contracts.WorkoutsContracts;

public class PostPutWorkoutRequest
{
    
    public DateTime CreatedAt { get; init; }

    
    
    public required string Title { get; init; }

    
    public WorkoutType Type { get; init; }

    
    public required List<PostPutExerciseRequest> Exercises { get; init; }

    
    
    public TimeSpan Duration { get; init; }

    
    public int CaloriesBurned { get; init; }

    
    public DateTime WorkoutDate { get; init; }
}