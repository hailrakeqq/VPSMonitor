using System.Net;
using Microsoft.AspNetCore.Mvc;
using Moq;
using VPSMonitor.API;
using VPSMonitor.API.Controllers;
using VPSMonitor.API.Entities;
using VPSMonitor.API.Repository;
using VPSMonitor.API.Services;

namespace VPSMonitor.Tests;

public class AuthControllerTest
{
    private readonly AuthController _authController;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ITokenService> _mockTokenService;

    public AuthControllerTest()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockTokenService = new Mock<ITokenService>();
        _authController = new AuthController(_mockUserRepository.Object,_mockTokenService.Object);
    }

    [Fact]
    public async Task CreateUser_ValidUser_ReturnsOkResult()
    {
        //Arrange
        var newUser = new User
        {
            Email = "test@example.com",
            Password = "password"
        };

        _mockUserRepository.Setup(repo => repo.GetUserByEmail(newUser.Email))
            .ReturnsAsync(null as User);
        
        // Act
        var result = await _authController.CreateUser(newUser) as OkObjectResult;
        
        // Assert
        Assert.NotNull(result);
        var createdUser = result.Value as User;
        Assert.NotNull(createdUser);
        Assert.NotNull(createdUser.Id);
        Assert.Equal(newUser.Email.ToLower(), createdUser.Email);
        Assert.Equal(Toolchain.GenerateHash(newUser.Password), createdUser.Password);
    }

    [Fact]
    public async Task CreateUser_ExistingUser_ReturnsConflictResult()
    {
        // Arrange
        var existingUser = new User
        {
            Email = "existinguser@test.com",
            Password = "123"
        };
        _mockUserRepository.Setup(repo => repo.GetUserByEmail(existingUser.Email))
            .ReturnsAsync(existingUser);
        
        // Act 
        var result = await _authController.CreateUser(existingUser) as ConflictObjectResult;
        
        // Assert 
        Assert.NotNull(result);
        Assert.Equal("user already exist.", result.Value);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkResult()
    {
        // Arrange
        var userLoginModel = new UserLoginModel
        {
            Email = "email@test.com",
            Password = "123456"
        };
        var currentUser = new User
        {
            Id = "user-id",
            Email = userLoginModel.Email
        };
        _mockUserRepository.Setup(repo => repo.ValidateUserLoginModel(userLoginModel))
            .ReturnsAsync(currentUser);
        _mockTokenService.Setup(service => service.GenerateRefreshToken(currentUser.Id))
            .Returns(new AuthRefreshToken());
        _mockTokenService.Setup(service => service.GenerateAccessToken(currentUser))
            .Returns("access-token");
        
        // Act
        var result = await _authController.Login(userLoginModel) as OkObjectResult;
        
        // Assert
        Assert.NotNull(result);
        var loginResponse = result.Value as LoginResponse;
        Assert.NotNull(loginResponse);
        Assert.Equal(currentUser.Id,loginResponse.Id);
        Assert.Equal(currentUser.Email, loginResponse.Email);
        Assert.Equal("access-token", loginResponse.AccessToken);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorizedResult()
    {
        // Arrange
        var userInvalidLoginModel = new UserLoginModel
        {
            Email = "invalid-mail@test.com",
            Password = "wrong-password"
        };
        var currentUser = null as User;
        _mockUserRepository.Setup(repo => repo.ValidateUserLoginModel(userInvalidLoginModel))
            .ReturnsAsync(currentUser);

        // Act
        var result = await _authController.Login(userInvalidLoginModel) as UnauthorizedObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal((int)HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_ValidRefreshToken_ReturnsOkResult()
    {
        // Arrange
        var userId = "user-id";
        var refreshToken = "refresh-token";
        var user = new User { Id = userId };
        var userRefreshTokenDocument = new AuthRefreshToken
        {
            RefreshToken = refreshToken,
            TokenExpires = DateTime.Now.AddMinutes(30)
        };

        _mockUserRepository.Setup(repo => repo.GetItemById(userId))
            .ReturnsAsync(user);
        _mockTokenService.Setup(service => service.GetRefreshTokenDocumentById(userId))
            .Returns(userRefreshTokenDocument);
        _mockTokenService.Setup(service => service.GenerateAccessToken(user))
            .Returns("new-access-token");
        
        // Act
        var result = await _authController.RefreshToken(userId, refreshToken) as OkObjectResult;
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("new-access-token", result.Value);
    }
}