using AutoMapper;
using FitnessTracker.DataAccess;
using FitnessTracker.DataAccess.Entities;
using FitnessTracker.DataAccess.Mapping;
using FitnessTracker.DataAccess.Repositories;
using FitnessTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace FitnessTacker.Tests.DataAccessTests;

public class UsersRepositoryTests
{
    private readonly IMapper _mapper;

    public UsersRepositoryTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserProfile>();     
        });

        _mapper = config.CreateMapper();
    }

    private FitnessTrackerDbContext CreateInMemoryDbContext(string dbName)
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            { "ConnectionStrings:PostgresConnectionString", "Host=localhost;Database=fake;Username=fake;Password=fake" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        var options = new DbContextOptionsBuilder<FitnessTrackerDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new FitnessTrackerDbContext(options, configuration);
    }

    [Fact]
    public async Task AddUserAsync_ShouldAddUser()
    {
        var context = CreateInMemoryDbContext(nameof(AddUserAsync_ShouldAddUser));
        var repository = new UsersRepository(context, _mapper);
        var user = User.Create("Test", "test@email.com", "hash");

        await repository.AddUserAsync(user, CancellationToken.None);

        var entity = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        Assert.NotNull(entity);
        Assert.Equal(user.Name, entity.Name);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsUser_WhenExists()
    {
        var context = CreateInMemoryDbContext(nameof(GetUserByEmailAsync_ReturnsUser_WhenExists));
        context.Users.Add(new UserEntity
        {
            Id = "1",
            Name = "John",
            Email = "john@example.com",
            PasswordHash = "hash"
        });
        await context.SaveChangesAsync();

        var repository = new UsersRepository(context, _mapper);
        var user = await repository.GetUserByEmailAsync("john@example.com", CancellationToken.None);

        Assert.NotNull(user);
        Assert.Equal("John", user.Name);
    }

    [Fact]
    public async Task GetUserByEmailAsync_ReturnsNull_WhenNotFound()
    {
        var context = CreateInMemoryDbContext(nameof(GetUserByEmailAsync_ReturnsNull_WhenNotFound));
        var repository = new UsersRepository(context, _mapper);

        var user = await repository.GetUserByEmailAsync("notfound@example.com", CancellationToken.None);

        Assert.Null(user);
    }
}