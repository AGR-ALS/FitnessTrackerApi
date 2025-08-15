using FitnessTracker.Domain.Abstractions.Services;
using FitnessTrackerApi.Contracts.UserContracts;
using FitnessTrackerApi.Controllers;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FitnessTacker.Tests.ApiTests.ControllersTests;

public class UsersControllerTests
{
    private readonly Mock<IUsersService> _usersServiceMock;
    private readonly Mock<IValidator<RegisterUserRequest>> _registerValidatorMock;
    private readonly Mock<IValidator<LoginUserRequest>> _loginValidatorMock;
    private readonly UsersController _controller;
    private readonly DefaultHttpContext _httpContext;

    public UsersControllerTests()
    {
        _usersServiceMock = new Mock<IUsersService>();
        _registerValidatorMock = new Mock<IValidator<RegisterUserRequest>>();
        _loginValidatorMock = new Mock<IValidator<LoginUserRequest>>();
        _controller = new UsersController(_usersServiceMock.Object);

        _httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = _httpContext
        };
    }

    [Fact]
    public async Task Register_CallsServiceAndReturnsOk()
    {
        var req = new RegisterUserRequest("user1", "user1@example.com", "pass");

        _registerValidatorMock
            .Setup(v => v.ValidateAsync(req, default))
            .ReturnsAsync(new ValidationResult());

        var result = await _controller.Register(req, _registerValidatorMock.Object, CancellationToken.None);

        _usersServiceMock.Verify(s =>
                s.RegisterUser("user1", "user1@example.com", "pass", CancellationToken.None),
            Times.Once);

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task Login_CallsService_SetsCookieAndReturnsOk()
    {
        var loginReq = new LoginUserRequest("login@example.com", "pwd");
        var fakeToken = "jwt-token-123";
        var fakeRefreshToken = "jwt-refresh-123";

        _loginValidatorMock
            .Setup(v => v.ValidateAsync(loginReq, default))
            .ReturnsAsync(new ValidationResult());

        _usersServiceMock
            .Setup(s => s.Login("login@example.com", "pwd", CancellationToken.None))
            .ReturnsAsync((fakeToken, fakeRefreshToken));

        var result = await _controller.Login(loginReq, _loginValidatorMock.Object, CancellationToken.None);

        _usersServiceMock.Verify(s =>
                s.Login("login@example.com", "pwd", CancellationToken.None),
            Times.Once);

        Assert.IsType<OkResult>(result);

        var setCookie = _httpContext.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains("access_token=", setCookie);
        Assert.Contains(fakeToken, setCookie);
    }
}