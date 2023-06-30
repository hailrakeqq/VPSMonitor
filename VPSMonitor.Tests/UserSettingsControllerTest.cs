using Microsoft.AspNetCore.Mvc;
using Moq;
using VPSMonitor.API;
using VPSMonitor.API.Controllers;
using VPSMonitor.API.Entities;
using VPSMonitor.API.Repository;

namespace VPSMonitor.Tests;

public class UserSettingsControllerTest
{
    private readonly UserSettingsController _userSettingsController;
    private readonly Mock<IUserRepository> _mockUserRepository;

    public UserSettingsControllerTest()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _userSettingsController = new UserSettingsController(_mockUserRepository.Object);
    }

    [Fact]
    public async Task ChangeUserData_ExistingUser_ReturnOkResult()
    {
        // Arrange
        string userId = "1";
        string existingPasswordHash = Toolchain.GenerateHash("oldpassword");
        string newEmail = "newEmail@example.com";
        string newPassword = "newPassword"; 

        User existingUser = new User()
        {
            Id = userId,
            Email = "test@email.com",
            Password = existingPasswordHash
        };

        UserChangeDataModel newUserData = new UserChangeDataModel()
        {
            ConfirmPassword = "oldpassword",
            Password = newPassword,
            Email = newEmail
        };

        _mockUserRepository.Setup(repo => repo.GetItemById(userId)).ReturnsAsync(existingUser);
        
        // Act
        var result = await _userSettingsController.ChangeUserData(userId, newUserData) as OkObjectResult;
        
        // Assert
        _mockUserRepository.Verify(repo => repo.GetItemById(userId), Times.Once);
        _mockUserRepository.Verify(repo => repo.Update(existingUser), Times.Once);
        Assert.NotNull(result);
        Assert.Equal("Your user data has been changed successfully.", result.Value);
        Assert.Equal(Toolchain.GenerateHash(newPassword), existingUser.Password);
        Assert.Equal(newEmail, existingUser.Email);
    }

    [Fact]
    public async Task ChangeUserData_ExistingUserPasswordThatUserAlreadyUse_ReturnConflictResult()
    {
        // Arrange
        string userId = "1";
        string existingPasswordHash = Toolchain.GenerateHash("oldpassword");
        string existingEmail = "email@example.com";
        string newPassword = "newPassword"; 
        
        User existingUser = new User()
        {
            Id = userId,
            Email = existingEmail,
            Password = existingPasswordHash
        };
        
        var newUserData = new UserChangeDataModel()
        {
            ConfirmPassword = "oldpassword",
            Email = existingEmail,
            Password = "oldpassword"
        };
        _mockUserRepository.Setup(repo => repo.GetItemById(userId)).ReturnsAsync(existingUser);
        // Act
        var result = await _userSettingsController.ChangeUserData(userId,newUserData) as ConflictObjectResult;
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("You cannot set the password you had before.", result.Value);
    }

    [Fact]
    public async Task DeleteUserAccount_ExistingUser_ReturnOkResult()
    {
        // Arrange
        string userId = "1";
        
        User existingUser = new User()
        {
            Id = userId,
            Email = "existingEmail@test.com",
            Password = Toolchain.GenerateHash("existingPasswordHash")
        };

        _mockUserRepository.Setup(repo => repo.GetItemById(userId)).ReturnsAsync(existingUser);
        
        // Act
        var result = await _userSettingsController.DeleteUserAccount(userId, "existingPasswordHash") as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal("User account was seccessfully deleted.", result.Value);
    }

    [Fact]
    public async Task DeleteUserAccount_NonExistingUser_ReturnNotFoundResult()
    {
        // Arrange
        var userId = "nonExistId";
        var confirmPassword = "test";
        _mockUserRepository.Setup(repo => repo.GetItemById(userId)).ReturnsAsync((User)null);
        
        // Act
        var result = await _userSettingsController.DeleteUserAccount(userId, confirmPassword) as NotFoundResult;
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(404, result.StatusCode);
    }
}