using AutoMapper;
using FitnessTracker.Business.Exceptions;
using FitnessTracker.Business.Services;
using FitnessTracker.DataAccess;
using FitnessTracker.DataAccess.Entities;
using FitnessTracker.DataAccess.Mapping;
using FitnessTracker.DataAccess.Repositories;
using FitnessTracker.Domain.Abstractions.Repositories;
using FitnessTracker.Domain.DataFilters;
using FitnessTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FitnessTacker.Tests.ServicesTests;

public class WorkoutServiceTests
{
    private readonly Mock<IWorkoutsRepository> _repositoryMock;
    private readonly WorkoutsService _service;

    public WorkoutServiceTests()
    {
        _repositoryMock = new Mock<IWorkoutsRepository>();
        _service = new WorkoutsService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetWorkoutByIdAsync_WorkoutExists_ReturnsWorkout()
    {
        var workout = new Workout { Id = "1", Title = "Test", CreatedAt = DateTime.UtcNow };
        _repositoryMock.Setup(r => r.GetWorkoutByIdAsync("1", CancellationToken.None)).ReturnsAsync(workout);

        var result = await _service.GetWorkoutByIdAsync("1", CancellationToken.None);

        Assert.Equal(workout, result);
    }

    [Fact]
    public async Task GetWorkoutByIdAsync_WorkoutNotFound_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetWorkoutByIdAsync("1", CancellationToken.None)).ReturnsAsync((Workout?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetWorkoutByIdAsync("1", CancellationToken.None));
    }

    [Fact]
    public async Task AddWorkoutAsync_SuccessfullyAddsWorkout()
    {
        var workout = new Workout { Id = "2" };

        await _service.AddWorkoutAsync(workout, CancellationToken.None);

        _repositoryMock.Verify(r => r.AddWorkoutAsync(workout, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteWorkoutAsync_SuccessfullyDeletesWorkout()
    {
        var id = Guid.NewGuid().ToString();

        await _service.DeleteWorkoutAsync(id, CancellationToken.None);

        _repositoryMock.Verify(r => r.DeleteWorkoutAsync(id, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateWorkoutAsync_SuccessfullyUpdatesWorkout()
    {
        var workout = new Workout { Id = "2" };

        await _service.UpdateWorkoutAsync(workout, CancellationToken.None);

        _repositoryMock.Verify(r => r.UpdateWorkoutAsync(workout, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateWorkoutAsync_WorkoutNotFound_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.UpdateWorkoutAsync(It.IsAny<Workout>(), CancellationToken.None))
            .Throws<InvalidOperationException>();

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateWorkoutAsync(new Workout(), CancellationToken.None));
    }

    [Fact]
    public async Task UpdateWorkoutProgressPhotosAsync_CallsRepository_WhenWorkoutExists()
    {
        var workout = new Workout { Id = "1" };
        _repositoryMock.Setup(r => r.UpdateWorkoutProgressPhotosAsync(workout, CancellationToken.None))
            .Returns(Task.CompletedTask);

        await _service.UpdateWorkoutProgressPhotosAsync(workout, CancellationToken.None);

        _repositoryMock.Verify(r => r.UpdateWorkoutProgressPhotosAsync(workout, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateWorkoutProgressPhotosAsync_WorkoutNotFound_ThrowsNotFoundException()
    {
        var workout = new Workout { Id = "bad" };
        _repositoryMock.Setup(r => r.UpdateWorkoutProgressPhotosAsync(workout, CancellationToken.None))
            .ThrowsAsync(new InvalidOperationException());

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateWorkoutProgressPhotosAsync(workout, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteWorkoutAsync_WorkoutNotFound_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.DeleteWorkoutAsync(It.IsAny<string>(), CancellationToken.None))
            .Throws<InvalidOperationException>();

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteWorkoutAsync("bad-id", CancellationToken.None));
    }

    public static IEnumerable<object[]> sortItmes = new List<object[]>
    {
        new object[] { "caloriesburned" },
        new object[] { "createdat" },
        new object[] { "id" },
    };

    [Theory]
    [MemberData(nameof(sortItmes))]
    public async Task GetWorkoutsAsync_WithFilters_AppliesFilteringAndSorting(string sortItem)
    {
        var context =
            CreateInMemoryDbContext(nameof(GetWorkoutsAsync_WithFilters_AppliesFilteringAndSorting) + sortItem);
        var config = new MapperConfiguration(cfg => { cfg.AddProfile<WorkoutProfile>(); });

        var mapper = config.CreateMapper();

        var w1 = new WorkoutEntity
        {
            Id = "1",
            Title = "Cardio Workout",
            UserId = "u1",
            Type = WorkoutType.Cardio,
            CreatedAt = DateTime.Today,
            Duration = TimeSpan.FromMinutes(30),
            ProgressPhotos = new List<string>()
        };
        var w2 = new WorkoutEntity
        {
            Id = "2",
            Title = "Strength Workout",
            UserId = "u2",
            Type = WorkoutType.Strength,
            CreatedAt = DateTime.Today.AddDays(-1),
            Duration = TimeSpan.FromMinutes(45),
            ProgressPhotos = new List<string>()
        };

        context.Workouts.AddRange(w1, w2);
        await context.SaveChangesAsync();

        var repository = new WorkoutsRepository(context, mapper);

        var filter = new WorkoutFilter(
            WorkoutType.Cardio,
            DateTime.Today,
            TimeSpan.FromMinutes(30),
            sortItem,
            "desc",
            PageNumber: 1,
            PageSize: 10
        );

        var result = await repository.GetWorkoutsAsync(filter, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("1", result[0].Id);
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
}