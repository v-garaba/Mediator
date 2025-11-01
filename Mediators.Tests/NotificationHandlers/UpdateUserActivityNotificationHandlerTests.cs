using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Mediators.NotificationHandlers.Tests;

[TestFixture]
internal sealed class UpdateUserActivityNotificationHandlerTests
{
    private readonly UserRef _userRef = new();

    [Test]
    public async Task UpdateUserActivity_UpdatesUserLastActiveTime()
    {
        // Arrange
        var user = new User(_userRef, "Alice", "alice@example.com", DateTimeOffset.UtcNow.AddHours(-5), UserStatus.Offline);

        IStorage<UserRef, User> userRepository = new UserStorage();
        await userRepository.SetAsync(user);

        UpdateUserActivityNotificationHandler notificationHandler = new(
            LoggerFactory
                .Create(builder => builder.SetMinimumLevel(LogLevel.None))
                .CreateLogger<UpdateUserActivityNotificationHandler>(),
            userRepository
        );

        var mediator = new ChatMediator([], [notificationHandler]);

        // Act
        var utcNowSnapshot = DateTimeOffset.UtcNow;
        await mediator.PublishAsync(new UpdateUserActivityNotification(_userRef));

        var storedUser = await userRepository.TryGetAsync(_userRef);

        // Assert
        Assert.That(storedUser, Is.Not.Null);
        Assert.That(storedUser!.LastActiveTime, Is.EqualTo(utcNowSnapshot).Within(TimeSpan.FromSeconds(1)));
    }
}
