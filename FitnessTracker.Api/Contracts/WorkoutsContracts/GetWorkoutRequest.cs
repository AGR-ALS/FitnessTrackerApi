using System.ComponentModel.DataAnnotations;
using FitnessTracker.Domain.Models;

namespace FitnessTrackerApi.Contracts.WorkoutsContracts;

public record GetWorkoutRequest
{
    public WorkoutType? Type { get; init; }
    public DateTime? CreatedAt { get; init; }
    
    
    public TimeSpan? Duration { get; init; }
    
    
    public string? SortItem { get; init; }
    
    public string? SortOrder { get; init; }
    
    
    public int PageNumber { get; init; }
    
    
    public int PageSize { get; init; }
    
}