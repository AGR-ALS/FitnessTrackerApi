using AutoMapper;
using FitnessTracker.DataAccess.Entities;
using FitnessTracker.Domain.Models;

namespace FitnessTracker.DataAccess.Mapping;

public class RefreshTokenProfile : Profile
{
    public RefreshTokenProfile()
    {
        CreateMap<RefreshTokenEntity, RefreshToken>();
        CreateMap<RefreshToken, RefreshTokenEntity>();
    }
}