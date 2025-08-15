using AutoMapper;
using FitnessTracker.DataAccess.Entities;
using FitnessTracker.DataAccess.Mapping;
using FitnessTracker.Domain.Models;

namespace FitnessTacker.Tests.DataAccessTests.Mappings;

public class AutoMapperProfilesTests
{
    private readonly MapperConfiguration _configuration;
    private readonly IMapper _mapper;
    public AutoMapperProfilesTests()
    {
        _configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();
            cfg.AddProfile<WorkoutProfile>();
        });
        _mapper = _configuration.CreateMapper();
    }

    [Fact]
    public void UserEntity_To_User_And_Back_Roundtrip()
    {
        var entity = new UserEntity
        {
            Id = "u1",
            Name = "Alice",
            Email = "alice@example.com",
            PasswordHash = "hash"
        };
        
        var domain = _mapper.Map<User>(entity);
        Assert.Equal(entity.Id, domain.Id);
        Assert.Equal(entity.Name, domain.Name);
        Assert.Equal(entity.Email, domain.Email);
        Assert.Equal(entity.PasswordHash, domain.PasswordHash);
        
        var back = _mapper.Map<UserEntity>(domain);
        Assert.Equal(domain.Id, back.Id);
        Assert.Equal(domain.Name, back.Name);
        Assert.Equal(domain.Email, back.Email);
        Assert.Equal(domain.PasswordHash, back.PasswordHash);
    }

    [Fact]
    public void WorkoutEntity_To_Workout_And_Back_Roundtrip()
    {
        var exerciseEntity = new ExerciseEntity
        {
            Id = "e1",
            Name = "Squat",
            Sets = new List<SetEntity>
            {
                new SetEntity { Id = "s1", Reps = 10, Weight = 100 }
            }
        };
        var userEntity = new UserEntity()
        {
            Id="u1"
        };
        var entity = new WorkoutEntity
        {
            Id = "w1",
            CreatedAt = new DateTime(2025, 8, 5, 10, 0, 0),
            UserId = "u1",
            User = userEntity,
            Title = "Leg Day",
            Type = WorkoutType.Strength,
            Exercises = new List<ExerciseEntity> { exerciseEntity },
            Duration = TimeSpan.FromMinutes(60),
            CaloriesBurned = 500,
            ProgressPhotos = new List<string> { "a.jpg" },
            WorkoutDate = new DateTime(2025, 8, 4)
        };
        
        var domain = _mapper.Map<Workout>(entity);
        Assert.Equal(entity.Id, domain.Id);
        Assert.Equal(entity.Title, domain.Title);
        Assert.Equal(entity.Exercises.Count, domain.Exercises.Count);
        Assert.Equal(entity.User.Id, domain.UserId);
        Assert.Equal(entity.Exercises[0].Name, domain.Exercises[0].Name);
        Assert.Equal(entity.Exercises[0].Sets[0].Reps, domain.Exercises[0].Sets[0].Reps);
        
        var back = _mapper.Map<WorkoutEntity>(domain);
        Assert.Equal(domain.Id, back.Id);
        Assert.Equal(domain.Title, back.Title);
        Assert.Equal(domain.Exercises.Count, back.Exercises.Count);
        Assert.Equal(domain.Exercises[0].Name, back.Exercises[0].Name);
        Assert.Equal(domain.Exercises[0].Sets[0].Weight, back.Exercises[0].Sets[0].Weight);
    }
}