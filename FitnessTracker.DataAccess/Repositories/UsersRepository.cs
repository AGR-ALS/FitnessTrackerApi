using AutoMapper;
using FitnessTracker.DataAccess.Entities;
using FitnessTracker.Domain.Abstractions.Repositories;
using FitnessTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.DataAccess.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly FitnessTrackerDbContext _dbContext;
    private readonly IMapper _mapper;

    public UsersRepository(FitnessTrackerDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken)
    {
        var userEntity = _mapper.Map<UserEntity>(user);
        await _dbContext.Users.AddAsync(userEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var userEntity = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u=>u.Email == email, cancellationToken);

        if (userEntity == null)
        {
            return null;
        }

        return _mapper.Map<User>(userEntity);
    }

    public async Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var userEntity = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u=> u.Id == userId, cancellationToken);
        if (userEntity == null)
        {
            return null;
        }
        return _mapper.Map<User>(userEntity);
    }
}