using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Mediators.NotificationHandlers.Tests;

[TestFixture]
internal sealed class UpdateUserStatusNotificationHandlerTests
{
    private readonly UserRef _userRef = new();

    [Test]
    public async Task UpdateUserStatus_UpdatesUserStatus()
    {
        // Arrange
        var user = new User(
            _userRef,
            "Alice",
            "alice@example.com",
            DateTimeOffset.UtcNow.AddHours(-5),
            UserStatus.Offline
        );

        var userRepository = new UserStorage();
        await userRepository.SetAsync(user);

        INotificationHandler<NotifyUserStatusChangeNotification> mockedNotifyHandler =
            new MockedNotifyHandler();

        using var serviceProvider = new ServiceCollection()
            .AddLogging(builder => builder.SetMinimumLevel(LogLevel.None))
            .AddSingleton<IStorage<UserRef, User>>(userRepository)
            .AddSingleton<INotificationHandler, UpdateUserStatusNotificationHandler>()
            .AddSingleton<INotificationHandler>(mockedNotifyHandler) // NOTE: Register the mocked handler!!!
            .AddSingleton<IMediator, ChatMediator>()
            .BuildServiceProvider();

        var mediator = serviceProvider.GetRequiredService<IMediator>();

        // Act
        await mediator.PublishAsync(new UpdateUserStatusNotification(_userRef, UserStatus.Online));

        var storedUser = await userRepository.TryGetAsync(_userRef);

        // Assert
        Assert.That(storedUser, Is.Not.Null);
        Assert.That(storedUser!.Status, Is.EqualTo(UserStatus.Online));

        var receivedNotifications = ((MockedNotifyHandler)mockedNotifyHandler).ReceivedNotifications;
        Assert.That(receivedNotifications, Has.Count.EqualTo(1));
        Assert.That(receivedNotifications[0].User.Id, Is.EqualTo(_userRef));
        Assert.That(receivedNotifications[0].OldStatus, Is.EqualTo(UserStatus.Offline));
        Assert.That(receivedNotifications[0].NewStatus, Is.EqualTo(UserStatus.Online));
    }

    private sealed class MockedNotifyHandler()
        : INotificationHandler<NotifyUserStatusChangeNotification>
    {
        public readonly List<NotifyUserStatusChangeNotification> ReceivedNotifications = new();

        public Task HandleAsync(NotifyUserStatusChangeNotification notification)
        {
            ReceivedNotifications.Add(notification);
            return Task.CompletedTask;
        }
    }
}
