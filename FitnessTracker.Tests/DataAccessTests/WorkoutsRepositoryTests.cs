using AutoMapper;
using FitnessTracker.DataAccess;
using FitnessTracker.DataAccess.Entities;
using FitnessTracker.DataAccess.Mapping;
using FitnessTracker.DataAccess.Repositories;
using FitnessTracker.Domain.DataFilters;
using FitnessTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FitnessTacker.Tests.DataAccessTests;

public class WorkoutsRepositoryTests
{
    private readonly IMapper _mapper;

    public WorkoutsRepositoryTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<WorkoutProfile>();
        });

        _mapper = config.CreateMapper();
    }

    private FitnessTrackerDbContext CreateInMemoryDbContext(string dbName)
    {
        var settings = new Dictionary<string, string>
        {
            { "ConnectionStrings:PostgresConnectionString", "Host=localhost;Database=mock;Username=mock;Password=mock" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var options = new DbContextOptionsBuilder<FitnessTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new FitnessTrackerDbContext(options, configuration);
    }

    private Workout CreateSampleWorkout(string id = null)
    {
        return Workout.RestoreFromEntity(
            id ?? Guid.NewGuid().ToString(),
            DateTime.UtcNow,
            "user123",
            "Test Workout",
            WorkoutType.Cardio,
            new List<Exercise>(),
            TimeSpan.FromMinutes(45),
            500,
            new List<string> { "photo1.jpg" },
            DateTime.Today
        );
    }

    [Fact]
    public async Task AddWorkoutAsync_AddsWorkout()
    {
        var context = CreateInMemoryDbContext(nameof(AddWorkoutAsync_AddsWorkout));
        var repository = new WorkoutsRepository(context, _mapper);
        var workout = CreateSampleWorkout();

        await repository.AddWorkoutAsync(workout, CancellationToken.None);

        Assert.Single(context.Workouts);
        Assert.Equal("Test Workout", context.Workouts.First().Title);
    }

    [Fact]
    public async Task GetWorkoutByIdAsync_ReturnsWorkout_WhenExists()
    {
        var context = CreateInMemoryDbContext(nameof(GetWorkoutByIdAsync_ReturnsWorkout_WhenExists));
        var workout = _mapper.Map<WorkoutEntity>(CreateSampleWorkout("123"));
        context.Workouts.Add(workout);
        await context.SaveChangesAsync();

        var repository = new WorkoutsRepository(context, _mapper);
        var result = await repository.GetWorkoutByIdAsync("123", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Test Workout", result.Title);
    }

    [Fact]
    public async Task UpdateWorkoutAsync_UpdatesWorkout()
    {
        var context = CreateInMemoryDbContext(nameof(UpdateWorkoutAsync_UpdatesWorkout));
        var workout = _mapper.Map<WorkoutEntity>(CreateSampleWorkout("update-id"));
        context.Workouts.Add(workout);
        await context.SaveChangesAsync();

        var repository = new WorkoutsRepository(context, _mapper);
        var updated = CreateSampleWorkout("update-id");
        updated.Title = "Updated Title";
        updated.CaloriesBurned = 999;

        await repository.UpdateWorkoutAsync(updated, CancellationToken.None);
        var result = await context.Workouts.FindAsync("update-id");

        Assert.Equal("Updated Title", result.Title);
        Assert.Equal(999, result.CaloriesBurned);
    }

    [Fact]
    public async Task DeleteWorkoutAsync_RemovesWorkout()
    {
        var context = CreateInMemoryDbContext(nameof(DeleteWorkoutAsync_RemovesWorkout));
        var workout = _mapper.Map<WorkoutEntity>(CreateSampleWorkout("delete-id"));
        context.Workouts.Add(workout);
        await context.SaveChangesAsync();

        var repository = new WorkoutsRepository(context, _mapper);
        await repository.DeleteWorkoutAsync("delete-id", CancellationToken.None);

        Assert.Empty(context.Workouts);
    }

    [Fact]
    public async Task UpdateWorkoutProgressPhotosAsync_UpdatesPhotos()
    {
        var context = CreateInMemoryDbContext(nameof(UpdateWorkoutProgressPhotosAsync_UpdatesPhotos));
        var workout = _mapper.Map<WorkoutEntity>(CreateSampleWorkout("photo-id"));
        context.Workouts.Add(workout);
        await context.SaveChangesAsync();

        var repository = new WorkoutsRepository(context, _mapper);
        var update = CreateSampleWorkout("photo-id");
        update.ProgressPhotos = new List<string> { "new_photo.jpg" };

        await repository.UpdateWorkoutProgressPhotosAsync(update, CancellationToken.None);
        var result = await context.Workouts.FindAsync("photo-id");

        Assert.Contains("new_photo.jpg", result.ProgressPhotos);
    }
    
    [Fact]
    public async Task GetWorkoutsAsync_ReturnsAllWorkouts()
    {
        
        var context = CreateInMemoryDbContext(nameof(GetWorkoutsAsync_ReturnsAllWorkouts));
        
        var w1 = _mapper.Map<WorkoutEntity>(CreateSampleWorkout("w1"));
        var w2 = _mapper.Map<WorkoutEntity>(CreateSampleWorkout("w2"));
        w2.Title = "Second Workout";
        context.Workouts.AddRange(w1, w2);
        await context.SaveChangesAsync();

        var repository = new WorkoutsRepository(context, _mapper);
        
        var filter = new WorkoutFilter(
            null, null, null, null, null,
            PageNumber: 1,
            PageSize: 10
        );
        
        var result = await repository.GetWorkoutsAsync(filter, CancellationToken.None);
        
        Assert.Equal(2, result.Count);
        Assert.Contains(result, w => w.Id == "w1" && w.Title == w1.Title);
        Assert.Contains(result, w => w.Id == "w2" && w.Title == "Second Workout");
    }

}