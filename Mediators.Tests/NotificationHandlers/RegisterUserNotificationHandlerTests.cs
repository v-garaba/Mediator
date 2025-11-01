using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Mediators.NotificationHandlers.Tests;

[TestFixture]
internal sealed class RegisterUserNotificationHandlerTests
{
    private readonly UserRef _userRef = new();

    [Test]
    public async Task RegisterUser_AddsUserToMemory()
    {
        // Arrange
        var user = new User(_userRef, "Alice", "alice@example.com", DateTimeOffset.UtcNow, UserStatus.Offline);

        IStorage<UserRef, User> userRepository = new UserStorage();

        RegisterUserNotificationHandler notificationHandler = new(
            LoggerFactory
                .Create(builder => builder.SetMinimumLevel(LogLevel.None))
                .CreateLogger<RegisterUserNotificationHandler>(),
            userRepository
        );

        var mediator = new ChatMediator([], [notificationHandler]);

        // Act
        await mediator.PublishAsync(new RegisterUserNotification(user));

        var storedUser = await userRepository.TryGetAsync(_userRef);

        // Assert
        Assert.That(storedUser, Is.Not.Null);
        Assert.That(storedUser!.Name, Is.EqualTo("Alice"));
    }

    [Test]
    public async Task RegisterUser_WhenUpdate_UpdatesUserInMemory()
    {
        // Arrange
        var user = new User(_userRef, "Alice", "alice@example.com", DateTimeOffset.UtcNow, UserStatus.Offline);

        IStorage<UserRef, User> userRepository = new UserStorage();
        await userRepository.SetAsync(user);

        RegisterUserNotificationHandler notificationHandler = new(
            LoggerFactory
                .Create(builder => builder.SetMinimumLevel(LogLevel.None))
                .CreateLogger<RegisterUserNotificationHandler>(),
            userRepository
        );

        var mediator = new ChatMediator([], [notificationHandler]);

        // Act
        user = user with { Name = "Alice Updated" };
        await mediator.PublishAsync(new RegisterUserNotification(user));

        var storedUser = await userRepository.TryGetAsync(_userRef);

        // Assert
        Assert.That(storedUser, Is.Not.Null);
        Assert.That(storedUser!.Name, Is.EqualTo("Alice Updated"));
    }
}
