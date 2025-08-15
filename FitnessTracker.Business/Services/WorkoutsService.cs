using FitnessTracker.Business.Exceptions;
using FitnessTracker.Domain.Abstractions.Repositories;
using FitnessTracker.Domain.Abstractions.Services;
using FitnessTracker.Domain.DataFilters;
using FitnessTracker.Domain.Models;

namespace FitnessTracker.Business.Services;

public class WorkoutsService : IWorkoutsService
{
    private readonly IWorkoutsRepository _workoutsRepository;

    public WorkoutsService(IWorkoutsRepository workoutsRepository)
    {
        _workoutsRepository = workoutsRepository;
    }

    public async Task<List<Workout>> GetWorkoutsAsync(WorkoutFilter filter, CancellationToken cancellationToken)
    {
        var workouts = await _workoutsRepository.GetWorkoutsAsync(filter, cancellationToken);

        return workouts;
    }

    public async Task<Workout?> GetWorkoutByIdAsync(string id, CancellationToken cancellationToken)
    {
        var workout = await _workoutsRepository.GetWorkoutByIdAsync(id, cancellationToken);
        if (workout is null)
        {
            throw new NotFoundException("No workout with provided id was found");
        }
        return workout;
    }

    public async Task AddWorkoutAsync(Workout workout, CancellationToken cancellationToken)
    {
        try
        {
            await _workoutsRepository.AddWorkoutAsync(workout, cancellationToken);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message);
        }
        
    }

    public async Task UpdateWorkoutAsync(Workout workout, CancellationToken cancellationToken)
    {
        try
        {
            await _workoutsRepository.UpdateWorkoutAsync(workout, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("No workout with provided id was found");
        }
        
    }

    public async Task UpdateWorkoutProgressPhotosAsync(Workout workout, CancellationToken cancellationToken)
    {
        try
        {
            await _workoutsRepository.UpdateWorkoutProgressPhotosAsync(workout, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("No workout with provided id was found");
        }
    }
    public async Task DeleteWorkoutAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            await _workoutsRepository.DeleteWorkoutAsync(id, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            throw new NotFoundException("No workout with provided id was found");
        }
        
    }
}