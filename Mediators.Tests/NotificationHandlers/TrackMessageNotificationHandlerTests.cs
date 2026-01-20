using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using Mediators.Repository.EntityFramework;
using Mediators.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Mediators.Notifications.Tests;

[TestFixture]
internal sealed class TrackMessageNotificationHandlerTests
{
    private ServiceProvider _serviceProvider = null!;

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = new ServiceCollection()
            .RegisterUserRepositories(MockConfiguration.Default)
            .AddInMemoryEntityFrameworkStorage($"TestDb_{Guid.NewGuid()}") // Unique in-memory DB per test
            .BuildServiceProvider();
    }

    [TearDown]
    public void TearDown()
    {
        _serviceProvider.Dispose();
    }
    
    [Test]
    public async Task TrackMessageNotification_IncreasesCount()
    {
        // Arrange
        var userId1 = new UserRef();
        var userId2 = new UserRef();

        var memory = _serviceProvider.GetRequiredService<IStorage<UserRef, UserNotification>>();

        TrackMessageNotificationHandler notificationHandler = new(
            LoggerFactory
                .Create(builder => builder.SetMinimumLevel(LogLevel.None))
                .CreateLogger<TrackMessageNotificationHandler>(),
            memory
        );

        var mediator = new ChatMediator([], [notificationHandler]);

        // Act
        await mediator.PublishAsync(new TrackMessageNotification(userId1, new MessageRef()));
        await mediator.PublishAsync(new TrackMessageNotification(userId1, new MessageRef()));
        await mediator.PublishAsync(new TrackMessageNotification(userId2, new MessageRef()));

        var user1Notification = await memory.TryGetAsync(userId1);
        var user2Notification = await memory.TryGetAsync(userId2);

        // Assert
        Assert.That(user1Notification?.MessageCount, Is.EqualTo(2));
        Assert.That(user2Notification?.MessageCount, Is.EqualTo(1));
    }
}


