﻿using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using backend.Services.Interfaces;
using backend.Controllers;
using backend.DTOs.User;
using backend.Exceptions;
using FluentValidation;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IValidator<UserDto>> _mockUserDtoValidator;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockUserDtoValidator = new Mock<IValidator<UserDto>>();
        _authController = new AuthController(_mockAuthService.Object, _mockUserDtoValidator.Object);
    }

    [Fact]
    public async Task Login_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto { email = "invalidUser", password = "validPassword" };

        _mockAuthService.Setup(service => service.LoginAsync(loginDto, It.IsAny<HttpResponse>()))
                        .ThrowsAsync(new UserNotFoundException("User not found"));

        // Act
        var result = await _authController.Login(loginDto);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("User not found", unauthorizedResult.Value);
    }

    [Fact]
    public async Task Login_PasswordMismatch_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto { email = "validUser", password = "wrongPassword" };

        _mockAuthService.Setup(service => service.LoginAsync(loginDto, It.IsAny<HttpResponse>()))
                        .ThrowsAsync(new PasswordMismatchException("Password is incorrect"));

        // Act
        var result = await _authController.Login(loginDto);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Password is incorrect", unauthorizedResult.Value);
    }

    [Fact]
    public async Task Login_Exception_ReturnsBadRequest()
    {
        // Arrange
        var loginDto = new LoginDto { email = "validUser", password = "validPassword" };

        _mockAuthService.Setup(service => service.LoginAsync(loginDto, It.IsAny<HttpResponse>()))
                        .ThrowsAsync(new Exception("An unexpected error occurred"));

        // Act
        var result = await _authController.Login(loginDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("An unexpected error occurred", badRequestResult.Value);
    }
}
