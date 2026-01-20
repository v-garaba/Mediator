using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Notifications;

public sealed class SendPushNotificationNotificationHandler(
    ILogger<SendPushNotificationNotificationHandler> logger)
    : AbstractNotificationHandler<SendPushNotificationNotification>
{
    private readonly ILogger<SendPushNotificationNotificationHandler> _logger =
        logger.AssertNotNull();

    public sealed override async Task HandleAsync(SendPushNotificationNotification notification)
    {
        await Task.Yield();

        // Simulate sending push notification
        _logger.LogInformation(
            $"[PUSH] To User: {notification.UserId}, Message: {notification.Message}"
        );
    }
}


