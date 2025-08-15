using System.Linq.Expressions;
using AutoMapper;
using FitnessTracker.DataAccess.Entities;
using FitnessTracker.Domain.Abstractions.Repositories;
using FitnessTracker.Domain.DataFilters;
using FitnessTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.DataAccess.Repositories;

public class WorkoutsRepository : IWorkoutsRepository
{
    private readonly FitnessTrackerDbContext _dbContext;
    private readonly IMapper _mapper;

    public WorkoutsRepository(FitnessTrackerDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<Workout>> GetWorkoutsAsync(WorkoutFilter filter, CancellationToken cancellationToken)
    {
        var workoutEntitiesQuery = _dbContext.Workouts.AsNoTracking().AsQueryable();
        if (filter.Type is not null)
        {
            workoutEntitiesQuery = workoutEntitiesQuery.Where(w => w.Type == filter.Type);
        }

        if (filter.CreatedAt is not null)
        {
            workoutEntitiesQuery = workoutEntitiesQuery.Where(w => w.CreatedAt.Date == filter.CreatedAt.Value.Date);
        }

        if (filter.Duration is not null)
        {
            workoutEntitiesQuery = workoutEntitiesQuery.Where(w => w.Duration == filter.Duration);
        }

        if (!string.IsNullOrEmpty(filter.SortItem))
        {
            workoutEntitiesQuery = filter.SortOrder == "desc"
                ? workoutEntitiesQuery.OrderByDescending(GetSelectorKey(filter.SortItem)).AsQueryable()
                : workoutEntitiesQuery.OrderBy(GetSelectorKey(filter.SortItem)).AsQueryable();
        }

        workoutEntitiesQuery = workoutEntitiesQuery.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).AsQueryable();

        
        var workoutEntities = await workoutEntitiesQuery.ToListAsync(cancellationToken);
        var workouts = _mapper.Map<List<Workout>>(workoutEntities);
        return workouts;
    }

    public async Task<Workout?> GetWorkoutByIdAsync(string id, CancellationToken cancellationToken)
    {
        var workoutEntity = await _dbContext.Workouts.FindAsync(id, cancellationToken);

        return workoutEntity is not null ? _mapper.Map<Workout>(workoutEntity) : null;
    }

    public async Task AddWorkoutAsync(Workout workout, CancellationToken cancellationToken)
    {
        var workoutEntity = _mapper.Map<WorkoutEntity>(workout);
        await _dbContext.Workouts.AddAsync(workoutEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateWorkoutAsync(Workout workout, CancellationToken cancellationToken)
    {
        var workoutEntity = await _dbContext.Workouts.Include(w => w.Exercises).ThenInclude(ex => ex.Sets)
            .FirstAsync(w => w.Id == workout.Id, cancellationToken);

        workoutEntity.Title = workout.Title;
        workoutEntity.Type = workout.Type;
        workoutEntity.Duration = workout.Duration;
        workoutEntity.CaloriesBurned = workout.CaloriesBurned;
        workoutEntity.WorkoutDate = workout.WorkoutDate;

        var newExercises = _mapper.Map<List<ExerciseEntity>>(workout.Exercises);
        workoutEntity.Exercises = newExercises;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateWorkoutProgressPhotosAsync(Workout workout, CancellationToken cancellationToken)
    {
        var workoutEntity = await _dbContext.Workouts.FirstAsync(w => w.Id == workout.Id);
        workoutEntity.ProgressPhotos = workout.ProgressPhotos;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteWorkoutAsync(string id, CancellationToken cancellationToken)
    {
        _dbContext.Workouts.Remove(await _dbContext.Workouts.FirstAsync(w => w.Id == id, cancellationToken));
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private Expression<Func<WorkoutEntity, object>> GetSelectorKey(string sortItem)
    {
        return sortItem?.ToLower() switch
        {
            "createdat" => w => w.CreatedAt,
            "caloriesburned" => w => w.CaloriesBurned,
            _ => w => w.Id
        };
    }
}