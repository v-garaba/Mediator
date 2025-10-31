using Mediators.Handlers;
using Mediators.Messaging;
using Mediators.Messaging.Notifications;
using Mediators.Models;
using Mediators.Services;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Mediators.Tests.Services;

public class UserManagementServiceTests
{
    private ChatMediator _mediator;

    [SetUp]
    public void Setup()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.None));
        var logger = loggerFactory.CreateLogger<UserManagementService>();
        _mediator = new ChatMediator();
        _ = new UserManagementService(_mediator, logger); // Turn the service on
    }

    [Test]
    public async Task RegisterUser_AddsUserToCollection()
    {
        // Arrange
        var userRef = new UserRef();
        var user = new User(userRef, "Alice", "alice@example.com", DateTimeOffset.UtcNow, UserStatus.Offline);

        // Act
        await _mediator.PublishAsync(new RegisterUserNotification(user));
        var retrievedUserResponse = await _mediator.SendRequestAsync(new GetUserRequest(userRef));

        // Assert
        Assert.That(retrievedUserResponse.User, Is.Not.Null);
        Assert.That(retrievedUserResponse.User!.Name, Is.EqualTo("Alice"));
    }

    [Test]
    public async Task GetUser_ReturnsNullForNonExistentUser()
    {
        // Arrange & Act
        var nonExistent = new UserRef();
        var userResp = await _mediator.SendRequestAsync(new GetUserRequest(nonExistent));

        // Assert
        Assert.That(userResp.User, Is.Null);
    }

    [Test]
    public async Task UpdateUserActivity_UpdatesLastActiveTime()
    {
        // Arrange
        var userRef = new UserRef();
        var user = new User(userRef, "Alice", "alice@example.com", DateTimeOffset.UtcNow, UserStatus.Offline);
        await _mediator.PublishAsync(new RegisterUserNotification(user));

        var originalTime = user.LastActiveTime;

        Thread.Sleep(10); // Small delay to ensure time difference

        // Act
        await _mediator.PublishAsync(new UpdateUserActivityNotification(userRef));
        var updatedUserResp = await _mediator.SendRequestAsync(new GetUserRequest(userRef));

        // Assert
        Assert.That(updatedUserResp.User, Is.Not.Null);
        Assert.That(updatedUserResp.User!.LastActiveTime, Is.GreaterThan(originalTime));
    }

    [Test]
    public async Task UpdateUserStatus_ChangesUserStatus()
    {
        // Arrange
        var userRef = new UserRef();
        var user = new User(userRef, "Alice", "alice@example.com", DateTimeOffset.UtcNow, UserStatus.Offline);
        await _mediator.PublishAsync(new RegisterUserNotification(user));

        // Act
        await _mediator.PublishAsync(new UpdateUserStatusNotification(userRef, UserStatus.Online));
        var updatedUserResp = await _mediator.SendRequestAsync(new GetUserRequest(userRef));

        // Assert
        Assert.That(updatedUserResp.User, Is.Not.Null);
        Assert.That(updatedUserResp.User!.Status, Is.EqualTo(UserStatus.Online));
    }
}
