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
        var userId = "user1";

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
        var user1 = "user1";
        var user2 = "user2";

        // Act
        _analyticsService.TrackMessageNotification(user1, "msg1");
        _analyticsService.TrackMessageNotification(user1, "msg2");
        _analyticsService.TrackMessageNotification(user2, "msg3");

        // Assert
        Assert.That(_analyticsService.GetMessageCount(user1), Is.EqualTo(2));
        Assert.That(_analyticsService.GetMessageCount(user2), Is.EqualTo(1));
    }
}
