using FitnessTracker.Domain.Models;

namespace FitnessTrackerApi.Contracts.WorkoutsContracts;

public record GetWorkoutResponse(
    string Id,
    DateTime CreatedAt,
    string UserId,
    string Title,
    WorkoutType Type,
    List<GetExerciseResponse> Exercises,
    TimeSpan Duration,
    int CaloriesBurned,
    List<string>? ProgressPhotos,
    DateTime WorkoutDate
);