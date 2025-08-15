using AutoMapper;
using FitnessTracker.Business.Abstractions.Authentication;
using FitnessTracker.Business.Abstractions.Files;
using FitnessTracker.Business.Abstractions.Files.ImageUploading;
using FitnessTracker.Domain.Abstractions.Services;
using FitnessTracker.Domain.DataFilters;
using FitnessTracker.Domain.Models;
using FitnessTrackerApi.Contracts.WorkoutsContracts;
using FitnessTrackerApi.Controllers;
using FitnessTrackerApi.Mapping;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FitnessTacker.Tests.ApiTests.ControllersTests;

public class WorkoutsControllerTests
{
    private readonly Mock<IWorkoutsService> _svcMock;
    private readonly Mock<IImageUploadingService> _uploadMock;
    private readonly Mock<ICurrentUserDataService> _userDataMock;
    private readonly Mock<IValidator<string>> _idQueryValidatorMock;
    private readonly Mock<IFileDeletingService> _deleteMock;
    private readonly IMapper _mapper;
    private readonly WorkoutsController _controller;

    public WorkoutsControllerTests()
    {
        _svcMock = new Mock<IWorkoutsService>();
        _uploadMock = new Mock<IImageUploadingService>();
        _userDataMock = new Mock<ICurrentUserDataService>();
        _deleteMock = new Mock<IFileDeletingService>();
        _idQueryValidatorMock = new Mock<IValidator<string>>();

        var config = new MapperConfiguration(cfg => { cfg.AddProfile<WorkoutProfile>(); });
        _mapper = config.CreateMapper();

        _userDataMock.Setup(u => u.Id).Returns("user-1");

        _controller = new WorkoutsController(
            _svcMock.Object,
            _uploadMock.Object,
            _userDataMock.Object,
            _mapper,
            _deleteMock.Object
        );
    }

    [Fact]
    public async Task GetWorkouts_ReturnsOkWithMappedList()
    {
        var domainList = new List<Workout>
        {
            Workout.RestoreFromEntity("1", DateTime.UtcNow, "u", "A", WorkoutType.Cardio, new(), TimeSpan.Zero, 0,
                new(), DateTime.Today),
            Workout.RestoreFromEntity("2", DateTime.UtcNow, "u", "B", WorkoutType.Strength, new(), TimeSpan.Zero, 0,
                new(), DateTime.Today)
        };
        _svcMock.Setup(s => s.GetWorkoutsAsync(It.IsAny<WorkoutFilter>(), CancellationToken.None))
            .ReturnsAsync(domainList);

        var req = new GetWorkoutRequest { PageNumber = 1, PageSize = 10 };
        var getWorkoutsValidatorMock = new Mock<IValidator<GetWorkoutRequest>>();
        getWorkoutsValidatorMock.Setup(v => v.ValidateAsync(req, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        var result = await _controller.GetWorkouts(req, getWorkoutsValidatorMock.Object, CancellationToken.None);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<List<GetWorkoutResponse>>(ok.Value);

        Assert.Equal(2, dto.Count);
        Assert.Contains(dto, x => x.Id == "1" && x.Title == "A");
    }

    [Fact]
    public async Task GetWorkoutById_ReturnsOkWithDto()
    {
        var workout = Workout.RestoreFromEntity("abc", DateTime.UtcNow, "u", "Test", WorkoutType.Cardio, new(),
            TimeSpan.Zero, 0, new(), DateTime.Today);
        _svcMock.Setup(s => s.GetWorkoutByIdAsync("abc", CancellationToken.None)).ReturnsAsync(workout);
        _idQueryValidatorMock.Setup(v => v.ValidateAsync("abc", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var result = await _controller.GetWorkoutById("abc", _idQueryValidatorMock.Object, CancellationToken.None);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var dto = Assert.IsType<GetWorkoutResponse>(ok.Value);

        Assert.Equal("abc", dto.Id);
        Assert.Equal("Test", dto.Title);
    }

    [Fact]
    public async Task PostWorkout_MapsAndReturnsId()
    {
        var req = new PostWorkoutRequest
        {
            CreatedAt = DateTime.UtcNow,
            Title = "T",
            Type = WorkoutType.Cardio,
            Exercises = new List<PostPutExerciseRequest>(),
            Duration = TimeSpan.FromMinutes(5),
            CaloriesBurned = 100,
            WorkoutDate = DateTime.Today
        };

        var postWorkoutsValidatorMock = new Mock<IValidator<PostWorkoutRequest>>();
        postWorkoutsValidatorMock.Setup(v => v.ValidateAsync(req, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var actionResult = await _controller.PostWorkout(req, postWorkoutsValidatorMock.Object, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
        var returnedId = Assert.IsType<string>(ok.Value);

        Assert.False(string.IsNullOrWhiteSpace(returnedId));
        _svcMock.Verify(s =>
                s.AddWorkoutAsync(It.Is<Workout>(w =>
                    w.Id == returnedId &&
                    w.UserId == "user-1" &&
                    w.Title == "T"
                ), CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteWorkout_DeletesWorkoutAndFiles()
    {
        var w = Workout.RestoreFromEntity("x", DateTime.UtcNow, "u", "W", WorkoutType.Cardio, new(), TimeSpan.Zero, 0,
            new List<string> { "p1", "p2" }, DateTime.Today);
        _svcMock.Setup(s => s.GetWorkoutByIdAsync("x", CancellationToken.None)).ReturnsAsync(w);

        _idQueryValidatorMock.Setup(v => v.ValidateAsync("x", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var result = await _controller.DeleteWorkout("x", _idQueryValidatorMock.Object, CancellationToken.None);
        Assert.IsType<OkResult>(result);

        _svcMock.Verify(s => s.DeleteWorkoutAsync("x", CancellationToken.None), Times.Once);
        _deleteMock.Verify(d =>
                d.DeleteFileAsync(It.Is<List<string>>(l => l.SequenceEqual(new[] { "p1", "p2" }))),
            Times.Once
        );
    }

    [Fact]
    public async Task AddImagesToWorkout_UploadsAndUpdatesPhotos()
    {
        _uploadMock.Setup(u => u.UploadImageAsync(It.IsAny<IEnumerable<IFormFileAdapter>>(), "uploads"))
            .ReturnsAsync(new List<string> { "u1" });

        var w = Workout.RestoreFromEntity("x", DateTime.UtcNow, "u", "W", WorkoutType.Cardio, new(), TimeSpan.Zero, 0,
            new List<string> { "old" }, DateTime.Today);
        _svcMock.Setup(s => s.GetWorkoutByIdAsync("x", CancellationToken.None)).ReturnsAsync(w);

        _idQueryValidatorMock.Setup(v => v.ValidateAsync("x", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var postImageValidatorMock = new Mock<IValidator<PostImagesRequest>>();
        var req = new PostImagesRequest { ProgressPhotos = new List<Microsoft.AspNetCore.Http.IFormFile>() };
        postImageValidatorMock.Setup(v => v.ValidateAsync(req, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var result =
            await _controller.AddImagesToWorkout("x", _idQueryValidatorMock.Object, req, postImageValidatorMock.Object, CancellationToken.None);

        Assert.IsType<OkResult>(result);
        _uploadMock.Verify(u =>
                u.UploadImageAsync(It.IsAny<IEnumerable<IFormFileAdapter>>(), "uploads"),
            Times.Once
        );
        _svcMock.Verify(s =>
                s.UpdateWorkoutProgressPhotosAsync(It.Is<Workout>(wk => wk.ProgressPhotos.Contains("u1")), CancellationToken.None),
            Times.Once
        );
    }

    [Fact]
    public async Task ReuploadImagesForWorkout_UploadsReplacesAndDeletesOld()
    {
        var id = Guid.NewGuid().ToString();
        var existing = Workout.RestoreFromEntity(
            id, DateTime.UtcNow, "user-1", "Title",
            WorkoutType.Cardio, new(), TimeSpan.Zero, 0,
            new List<string> { "old1.jpg", "old2.jpg" },
            DateTime.Today);
        _svcMock.Setup(s => s.GetWorkoutByIdAsync(id, CancellationToken.None))
            .ReturnsAsync(existing);

        var files = new List<Microsoft.AspNetCore.Http.IFormFile>();
        _uploadMock
            .Setup(
                u => u.UploadImageAsync(It.IsAny<IEnumerable<FitnessTracker.Infrastructure.Files.FormFileAdapter>>(),
                    "uploads"))
            .ReturnsAsync(new List<string> { "new1.jpg", "new2.jpg" });

        _idQueryValidatorMock.Setup(v => v.ValidateAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var req = new PutImagesRequest { ProgressPhotos = files };

        var result = await _controller.ReuploadImagesForWorkout(id, _idQueryValidatorMock.Object, req, CancellationToken.None);

        Assert.IsType<OkResult>(result);
        _uploadMock.Verify(u => u.UploadImageAsync(
            It.Is<IEnumerable<FitnessTracker.Infrastructure.Files.FormFileAdapter>>(ad =>
                ad.Count() == files.Count), "uploads"), Times.Once);

        _svcMock.Verify(s =>
            s.UpdateWorkoutProgressPhotosAsync(It.Is<Workout>(w =>
                w.Id == id &&
                w.ProgressPhotos.SequenceEqual(new[] { "new1.jpg", "new2.jpg" })
            ), CancellationToken.None), Times.Once);

        _deleteMock.Verify(d =>
            d.DeleteFileAsync(It.Is<List<string>>(l =>
                l.SequenceEqual(new[] { "old1.jpg", "old2.jpg" }))), Times.Once);
    }

    [Fact]
    public async Task PutWorkout_MapsIdAndUserIdAndCallsService()
    {
        var id = Guid.NewGuid().ToString();
        var req = new PutWorkoutRequest
        {
            CreatedAt = DateTime.UtcNow,
            Title = "Updated",
            Type = WorkoutType.Strength,
            Exercises = new List<PostPutExerciseRequest>(),
            Duration = TimeSpan.FromMinutes(10),
            CaloriesBurned = 200,
            WorkoutDate = DateTime.Today
        };

        _idQueryValidatorMock.Setup(v => v.ValidateAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var putWorkoutsValidatorMock = new Mock<IValidator<PutWorkoutRequest>>();
        putWorkoutsValidatorMock.Setup(v => v.ValidateAsync(req, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        var result = await _controller.PutWorkout(id, _idQueryValidatorMock.Object, req,
            putWorkoutsValidatorMock.Object, CancellationToken.None);

        Assert.IsType<OkResult>(result);
        _svcMock.Verify(s =>
            s.UpdateWorkoutAsync(It.Is<Workout>(w =>
                w.Id == id &&
                w.UserId == "user-1" &&
                w.Title == "Updated" &&
                w.Type == WorkoutType.Strength
            ), CancellationToken.None), Times.Once);
    }
}