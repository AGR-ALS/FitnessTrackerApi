using FitnessTracker.Domain.DataFilters;
using FitnessTracker.Domain.Models;

namespace FitnessTracker.Domain.Abstractions.Repositories;

public interface IWorkoutsRepository
{
    Task<List<Workout>> GetWorkoutsAsync(WorkoutFilter filter, CancellationToken cancellationToken);
    Task<Workout?> GetWorkoutByIdAsync(string id, CancellationToken cancellationToken);
    Task AddWorkoutAsync(Workout workout, CancellationToken cancellationToken);
    Task UpdateWorkoutAsync(Workout workout, CancellationToken cancellationToken);
    Task UpdateWorkoutProgressPhotosAsync(Workout workout, CancellationToken cancellationToken);
    Task DeleteWorkoutAsync(string id, CancellationToken cancellationToken);
}