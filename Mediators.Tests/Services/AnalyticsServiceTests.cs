using Mediators.Handlers;
using Mediators.Messaging;
using Mediators.Messaging.Notifications;
using Mediators.Models;
using Mediators.Services;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Mediators.Tests.Services;

public class AnalyticsServiceTests
{
    private ChatMediator _mediator;

    [SetUp]
    public void Setup()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.None));
        var logger = loggerFactory.CreateLogger<AnalyticsService>();
        _mediator = new ChatMediator();
        _ = new AnalyticsService(_mediator, logger); // Turn the service on
    }

    [Test]
    public async Task TrackMessageNotification_IncreasesCount()
    {
        // Arrange
        var userId1 = new UserRef();
        var userId2 = new UserRef();

        List<(UserRef UserId, MessageRef MessageId)> trackedMessages = [];
        _mediator.Subscribe<TrackMessageNotification>(notification =>
        {
            trackedMessages.Add((notification.UserId, notification.MessageId));
            return Task.CompletedTask;
        });

        // Act
        await _mediator.PublishAsync(new TrackMessageNotification(userId1, new MessageRef()));
        await _mediator.PublishAsync(new TrackMessageNotification(userId1, new MessageRef()));
        await _mediator.PublishAsync(new TrackMessageNotification(userId2, new MessageRef()));

        var resp1 = await _mediator.SendRequestAsync(new GetMessageCountRequest(userId1));
        var resp2 = await _mediator.SendRequestAsync(new GetMessageCountRequest(userId2));

        // Assert
        Assert.That(trackedMessages.Count, Is.EqualTo(3));
        Assert.That(resp1.Count, Is.EqualTo(2));
        Assert.That(resp2.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetMessageCount_ReturnsZeroForNewUser()
    {
        // Arrange & Act
        var userId = new UserRef();
        var resp = await _mediator.SendRequestAsync(new GetMessageCountRequest(userId));

        // Assert
        Assert.That(resp.Count, Is.EqualTo(0));
    }
}
