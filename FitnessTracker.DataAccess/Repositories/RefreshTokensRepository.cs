using AutoMapper;
using FitnessTracker.DataAccess.Entities;
using FitnessTracker.Domain.Abstractions.Repositories;
using FitnessTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.DataAccess.Repositories;

public class RefreshTokensRepository : IRefreshTokensRepository
{
    private readonly IMapper _mapper;
    private readonly FitnessTrackerDbContext _dbContext;

    public RefreshTokensRepository(IMapper mapper, FitnessTrackerDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }
    
    public async Task<string> AddRefreshToken(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        var refreshTokenEntity = _mapper.Map<RefreshTokenEntity>(refreshToken);
        await _dbContext.AddAsync(refreshTokenEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return refreshTokenEntity.Token;
    }

    public async Task DeleteRefreshToken(string token, CancellationToken cancellationToken)
    {
        _dbContext.RemoveRange(await _dbContext.RefreshTokens.FirstAsync(r=> r.Token == token, cancellationToken));
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken> GetRefreshToken(string token, CancellationToken cancellationToken)
    {
        var tokenEntity = await _dbContext.RefreshTokens.AsNoTracking().FirstAsync(t=>t.Token == token, cancellationToken);
        return _mapper.Map<RefreshToken>(tokenEntity);
    }
}