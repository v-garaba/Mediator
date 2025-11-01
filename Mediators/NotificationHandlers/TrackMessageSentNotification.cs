using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.NotificationHandlers;

public sealed record TrackMessageSentNotification(UserRef UserId, string MessageType)
    : INotification;

public sealed class TrackMessageSentNotificationHandler(
    ILogger<TrackMessageSentNotificationHandler> logger
) : INotificationHandler<TrackMessageSentNotification>
{
    private readonly ILogger<TrackMessageSentNotificationHandler> _logger = logger.AssertNotNull();

    public async Task HandleAsync(TrackMessageSentNotification notification)
    {
        await Task.Yield();
        notification.AssertNotNull();
        _logger.LogInformation(
            $"[ANALYTICS] Message sent tracked for user {notification.UserId}, type: {notification.MessageType}"
        );
    }
}
