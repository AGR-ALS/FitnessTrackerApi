using AutoMapper;
using FitnessTracker.DataAccess.Entities;
using FitnessTracker.Domain.Models;

namespace FitnessTracker.DataAccess.Mapping;

public class WorkoutProfile : Profile
{
    public WorkoutProfile()
    {
        CreateMap<WorkoutEntity, Workout>();
        CreateMap<ExerciseEntity, Exercise>();
        CreateMap<SetEntity, Set>();
        CreateMap<Workout, WorkoutEntity>();
        CreateMap<Exercise, ExerciseEntity>();
        CreateMap<Set, SetEntity>();
    }
}