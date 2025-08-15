using FitnessTracker.Domain.Abstractions.Services;
using FitnessTrackerApi.Contracts.UserContracts;
using FitnessTrackerApi.Contracts.UserContracts.RefreshToken;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTrackerApi.Controllers;

[ApiController]
[Route("/users")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody]RegisterUserRequest request, [FromServices] IValidator<RegisterUserRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken: cancellationToken);
        await _usersService.RegisterUser(request.Username, request.Email, request.Password, cancellationToken);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody]LoginUserRequest loginUserRequest, [FromServices] IValidator<LoginUserRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(loginUserRequest, cancellationToken: cancellationToken);
        var (accessToken, refreshToken) = await _usersService.Login(loginUserRequest.Email, loginUserRequest.Password, cancellationToken);
        AddTokenToCookie("access_token", accessToken);
        AddTokenToCookie("refresh_token", refreshToken);
        return Ok();
    }

    [HttpPost("validate-refresh-token")]
    public async Task<ActionResult> LoginWithRefreshToken([FromServices] IRefreshTokensService refreshTokensService , CancellationToken cancellationToken)
    {
        string refreshToken = Request.Cookies["refresh_token"] ?? throw new UnauthorizedAccessException();
        var accessToken = await _usersService.Login(refreshToken, cancellationToken);
        AddTokenToCookie("access_token", accessToken);
        return Ok();
    }

    private void AddTokenToCookie(string tokenName, string token)
    {
        HttpContext.Response.Cookies.Append(tokenName, token);
    }
}