using Mediators.Messaging.Notifications;
using Mediators.Models;
using Mediators.Services;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Mediators.Tests.Services;

public class AnalyticsServiceTests
{
    private AnalyticsService _analyticsService = null!;

    [SetUp]
    public void Setup()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.None));
        var logger = loggerFactory.CreateLogger<AnalyticsService>();
        _analyticsService = new AnalyticsService(logger);
    }

    [Test]
    public void TrackMessageNotification_IncreasesCount()
    {
        // Arrange
        var userId = new UserRef();

        // Act
        _analyticsService.TrackMessageNotification(userId, "msg1");
        _analyticsService.TrackMessageNotification(userId, "msg2");

        // Assert
        Assert.That(_analyticsService.GetMessageCount(userId), Is.EqualTo(2));
    }

    [Test]
    public void GetMessageCount_ReturnsZeroForNewUser()
    {
        // Arrange & Act
        var count = _analyticsService.GetMessageCount("newUser");

        // Assert
        Assert.That(count, Is.EqualTo(0));
    }

    [Test]
    public void TrackUserStatusChange_DoesNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _analyticsService.TrackUserStatusChange("user1", "Online"));
    }

    [Test]
    public void TrackMessageSent_DoesNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _analyticsService.TrackMessageSent("user1", "Public"));
    }

    [Test]
    public void MultipleUsers_TrackSeparately()
    {
        // Arrange
        var user1 = new UserRef();
        var user2 = new UserRef();

        // Act
        _analyticsService.TrackMessageNotification(new TrackMessageNotification(user1, new MessageRef()));
        _analyticsService.TrackMessageNotification(new TrackMessageNotification(user1, new MessageRef()));
        _analyticsService.TrackMessageNotification(new TrackMessageNotification(user1, new MessageRef()));

        // Assert
        Assert.That(_analyticsService.GetMessageCount(new Mes user1), Is.EqualTo(2));
        Assert.That(_analyticsService.GetMessageCount(user2), Is.EqualTo(1));
    }
}
