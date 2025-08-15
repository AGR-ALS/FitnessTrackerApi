using System.ComponentModel.DataAnnotations;
using AutoMapper;
using FitnessTracker.Business.Abstractions.Authentication;
using FitnessTracker.Business.Abstractions.Files;
using FitnessTracker.Business.Abstractions.Files.ImageUploading;
using FitnessTracker.Domain.Abstractions.Services;
using FitnessTracker.Domain.DataFilters;
using FitnessTracker.Domain.Models;
using FitnessTracker.Infrastructure.Files;
using FitnessTrackerApi.Contracts.WorkoutsContracts;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrackerApi.Controllers;

[ApiController]
[Route("/workouts")]
public class WorkoutsController : ControllerBase
{
    private readonly IWorkoutsService _workoutsService;
    private readonly IImageUploadingService _imageUploadingService;
    private readonly ICurrentUserDataService _currentUserDataService;
    private readonly IMapper _mapper;
    private readonly IFileDeletingService _fileDeletingService;

    public WorkoutsController(IWorkoutsService workoutsService, IImageUploadingService imageUploadingService,
        ICurrentUserDataService currentUserDataService, IMapper mapper, IFileDeletingService fileDeletingService)
    {
        _workoutsService = workoutsService;
        _imageUploadingService = imageUploadingService;
        _currentUserDataService = currentUserDataService;
        _mapper = mapper;
        _fileDeletingService = fileDeletingService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<GetWorkoutResponse>>> GetWorkouts([FromQuery] GetWorkoutRequest request, IValidator<GetWorkoutRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
        var workouts = await _workoutsService.GetWorkoutsAsync(_mapper.Map<WorkoutFilter>(request), cancellationToken);
        var workoutResponse = _mapper.Map<List<GetWorkoutResponse>>(workouts);
        return Ok(workoutResponse);
    }

    [HttpGet("by-id")]
    [Authorize]
    public async Task<ActionResult<GetWorkoutResponse>> GetWorkoutById(
        [FromQuery] string id, [FromServices] IValidator<string> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(id, cancellationToken: cancellationToken);
        var workout = await _workoutsService.GetWorkoutByIdAsync(id, cancellationToken);
        var workoutResponse = _mapper.Map<GetWorkoutResponse>(workout);
        return Ok(workoutResponse);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<string>> PostWorkout([FromBody] PostWorkoutRequest request, [FromServices] IValidator<PostWorkoutRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
        var workout = _mapper.Map<Workout>(request, opts => { opts.Items["UserId"] = _currentUserDataService.Id; });
        await _workoutsService.AddWorkoutAsync(workout, cancellationToken);
        return Ok(workout.Id);
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult> PutWorkout(
        [FromQuery] string id, [FromServices] IValidator<string> idValidator,

    [FromBody] PutWorkoutRequest request, [FromServices] IValidator<PutWorkoutRequest> validator, CancellationToken cancellationToken)
    {
        await idValidator.ValidateAndThrowAsync(id, cancellationToken: cancellationToken);
        await validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
        var workout = _mapper.Map<Workout>(request, opts =>
        {
            opts.Items["Id"] = id;
            opts.Items["UserId"] = _currentUserDataService.Id;
        });
        await _workoutsService.UpdateWorkoutAsync(workout, cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [Authorize]
    public async Task<ActionResult> DeleteWorkout(
        [FromQuery] string id, [FromServices] IValidator<string> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(id, cancellationToken: cancellationToken);
        var progressPhotosToDelete =
            new List<string>(((await _workoutsService.GetWorkoutByIdAsync(id, cancellationToken))!).ProgressPhotos);
        await _workoutsService.DeleteWorkoutAsync(id, cancellationToken);
        await _fileDeletingService.DeleteFileAsync(progressPhotosToDelete);
        return Ok();
    }

    [HttpPost("upload-new-images")]
    [Authorize]
    public async Task<ActionResult> AddImagesToWorkout(
        [FromQuery] string id, [FromServices] IValidator<string> idValidator,

    [FromForm] PostImagesRequest request, [FromServices] IValidator<PostImagesRequest> validator, CancellationToken cancellationToken)
    {
        await idValidator.ValidateAndThrowAsync(id, cancellationToken: cancellationToken);
        await validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
        var progressImages =
            await _imageUploadingService.UploadImageAsync(request.ProgressPhotos.Select(p => new FormFileAdapter(p))!);
        var workout = await _workoutsService.GetWorkoutByIdAsync(id, cancellationToken);
        workout!.ProgressPhotos.AddRange(progressImages);
        await _workoutsService.UpdateWorkoutProgressPhotosAsync(workout, cancellationToken);
        return Ok();
    }

    [HttpPut("reupload-images")]
    [Authorize]
    public async Task<ActionResult> ReuploadImagesForWorkout(
        [FromQuery] string id, [FromServices] IValidator<string> idValidator,
        [FromForm] PutImagesRequest request, CancellationToken cancellationToken)
    {
        await idValidator.ValidateAndThrowAsync(id, cancellationToken: cancellationToken);
        var progressImages =
            await _imageUploadingService.UploadImageAsync(request.ProgressPhotos?.Select(p => new FormFileAdapter(p))!);
        var workout = await _workoutsService.GetWorkoutByIdAsync(id, cancellationToken);
        var progressPhotosToDelete =
            new List<string>(workout!.ProgressPhotos);
        workout!.ProgressPhotos = progressImages;
        await _workoutsService.UpdateWorkoutProgressPhotosAsync(workout, cancellationToken);
        await _fileDeletingService.DeleteFileAsync(progressPhotosToDelete);
        return Ok();
    }
}