using FitnessTracker.DataAccess.Configurations;
using FitnessTracker.DataAccess.Entities;
using FitnessTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WorkoutEntity = FitnessTracker.DataAccess.Entities.WorkoutEntity;

namespace FitnessTracker.DataAccess;

public class FitnessTrackerDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public FitnessTrackerDbContext(DbContextOptions<FitnessTrackerDbContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }
    public DbSet<WorkoutEntity> Workouts { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgresConnectionString"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new WorkoutsConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
    }
}