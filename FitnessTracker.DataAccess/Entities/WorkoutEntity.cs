using FitnessTracker.Domain.Models;

namespace FitnessTracker.DataAccess.Entities;

public class WorkoutEntity
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string UserId { get; set; }
    public UserEntity User { get; set; }
    public string Title { get; set; }
    public WorkoutType Type { get; set; }
    public List<ExerciseEntity> Exercises { get; set; } = [];
    public TimeSpan Duration { get; set; }
    public int CaloriesBurned { get; set; }
    public List<string>? ProgressPhotos { get; set; }
    public DateTime WorkoutDate { get; set; }
}