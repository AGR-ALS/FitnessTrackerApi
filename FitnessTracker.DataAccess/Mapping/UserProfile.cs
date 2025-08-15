using AutoMapper;
using FitnessTracker.DataAccess.Entities;
using FitnessTracker.Domain.Models;

namespace FitnessTracker.DataAccess.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserEntity, User>();
        CreateMap<User, UserEntity>();
    }
}