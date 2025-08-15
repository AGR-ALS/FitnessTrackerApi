using AutoMapper;
using FitnessTracker.Domain.DataFilters;
using FitnessTracker.Domain.Models;
using FitnessTrackerApi.Contracts.WorkoutsContracts;

namespace FitnessTrackerApi.Mapping;

public class WorkoutProfile : Profile
{
    public WorkoutProfile()
    {
        CreateMap<Workout, GetWorkoutResponse>();
        CreateMap<PostWorkoutRequest, Workout>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid().ToString()))
            .ForMember(dest => dest.ProgressPhotos, opt => opt.MapFrom(_ => new List<string>()))
            .ForMember(dest => dest.UserId,
                opt => opt.MapFrom((src, dest, destMember, context) => (string)context.Items["UserId"]));
        CreateMap<PutWorkoutRequest, Workout>()
            .ForMember(dest => dest.Id,
                opt => opt.MapFrom((src, dest, destMember, context) => (string)context.Items["Id"]))
            .ForMember(dest => dest.ProgressPhotos, opt => opt.MapFrom(_ => new List<string>()))
            .ForMember(dest => dest.UserId,
                opt => opt.MapFrom((src, dest, destMember, context) => (string)context.Items["UserId"]));
        CreateMap<PostPutExerciseRequest, Exercise>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid().ToString()));
        CreateMap<PostPutSetRequest, Set>().ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid().ToString()));
        CreateMap<GetWorkoutRequest, WorkoutFilter>();
        CreateMap<Exercise, GetExerciseResponse>();
        CreateMap<Set, GetSetResponse>();
    }
}