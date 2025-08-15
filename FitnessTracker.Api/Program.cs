using FitnessTracker.Business.Abstractions.Authentication;
using FitnessTracker.Business.Abstractions.Authentication.Jwt;
using FitnessTracker.Business.Abstractions.Files;
using FitnessTracker.Business.Abstractions.Files.ImageUploading;
using FitnessTracker.Business.Services;
using FitnessTracker.DataAccess;
using FitnessTracker.DataAccess.Mapping;
using FitnessTracker.DataAccess.Repositories;
using FitnessTracker.Domain.Abstractions.Repositories;
using FitnessTracker.Domain.Abstractions.Services;
using FitnessTracker.Infrastructure;
using FitnessTracker.Infrastructure.Authentication;
using FitnessTracker.Infrastructure.Authentication.Jwt;
using FitnessTracker.Infrastructure.Files;
using FitnessTrackerApi.Extensions;
using FitnessTrackerApi.Middleware;
using FluentValidation;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;

var options = new WebApplicationOptions
{
    WebRootPath = "wwwroot"
};
var builder = WebApplication.CreateBuilder(options);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<FitnessTrackerDbContext>(); 

builder.Services.AddAuthenticationWithJwtScheme(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IWorkoutsRepository, WorkoutsRepository>();
builder.Services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();

builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IWorkoutsService, WorkoutsService>();
builder.Services.AddScoped<IRefreshTokensService, RefreshTokensesService>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();

builder.Services.AddSingleton<IImageUploader, ImageUploader>();
builder.Services.AddSingleton<IImageUploadingService, ImageUploadingService>();
builder.Services.AddScoped<IFileDeletingService, FileDeletingService>();

builder.Services.AddAutoMapper(typeof(FitnessTrackerApi.Mapping.WorkoutProfile),
    typeof(FitnessTracker.DataAccess.Mapping.WorkoutProfile), typeof(UserProfile));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserDataService, CurrentUserDataService>();



var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<FitnessTrackerDbContext>();
        db.Database.Migrate();
    }
}

app.UseMiddleware<ExceptionHandler>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always,
});
app.UseAuthentication();
app.UseAuthorization();

app.Run();