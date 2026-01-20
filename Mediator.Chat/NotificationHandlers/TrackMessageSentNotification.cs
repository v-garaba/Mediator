using Microsoft.Extensions.Logging;

namespace Mediators.Notifications;

public sealed class TrackMessageSentNotificationHandler(
    ILogger<TrackMessageSentNotificationHandler> logger
) : AbstractNotificationHandler<TrackMessageSentNotification>
{
    private readonly ILogger<TrackMessageSentNotificationHandler> _logger = logger.AssertNotNull();

    public sealed override async Task HandleAsync(TrackMessageSentNotification notification)
    {
        await Task.Yield();
        notification.AssertNotNull();
        _logger.LogInformation(
            $"[ANALYTICS] Message sent tracked for user {notification.UserId}, type: {notification.MessageType}"
        );
    }
}


