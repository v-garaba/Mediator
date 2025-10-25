using Mediators.Models;
using Mediators.Services;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Mediators.Tests.Services;

public class UserManagementServiceTests
{
    private UserManagementService _userManagementService = null!;

    [SetUp]
    public void Setup()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.None));
        var logger = loggerFactory.CreateLogger<UserManagementService>();
        _userManagementService = new UserManagementService(logger);
    }

    [Test]
    public void RegisterUser_AddsUserToCollection()
    {
        // Arrange
        var user = new User("1", "Alice", "alice@example.com", DateTime.MinValue, UserStatus.Offline);

        // Act
        _userManagementService.RegisterUser(user);
        var retrievedUser = _userManagementService.GetUser("1");

        // Assert
        Assert.That(retrievedUser, Is.Not.Null);
        Assert.That(retrievedUser!.Name, Is.EqualTo("Alice"));
    }

    [Test]
    public void GetUser_ReturnsNullForNonExistentUser()
    {
        // Arrange & Act
        var user = _userManagementService.GetUser("nonexistent");

        // Assert
        Assert.That(user, Is.Null);
    }

    [Test]
    public void UpdateUserActivity_UpdatesLastActiveTime()
    {
        // Arrange
        var user = new User("1", "Alice", "alice@example.com", DateTime.MinValue, UserStatus.Offline);
        _userManagementService.RegisterUser(user);
        var originalTime = user.LastActiveTime;

        Thread.Sleep(10); // Small delay to ensure time difference

        // Act
        _userManagementService.UpdateUserActivity("1");
        var updatedUser = _userManagementService.GetUser("1");

        // Assert
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser!.LastActiveTime, Is.GreaterThan(originalTime));
    }

    [Test]
    public void UpdateUserStatus_ChangesUserStatus()
    {
        // Arrange
        var user = new User("1", "Alice", "alice@example.com", DateTime.MinValue, UserStatus.Offline);
        _userManagementService.RegisterUser(user);

        // Act
        _userManagementService.UpdateUserStatus("1", UserStatus.Online);
        var updatedUser = _userManagementService.GetUser("1");

        // Assert
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser!.Status, Is.EqualTo(UserStatus.Online));
    }

    [Test]
    public void UpdateUserActivity_ForNonExistentUser_DoesNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _userManagementService.UpdateUserActivity("nonexistent"));
    }
}
