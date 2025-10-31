using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Messaging.Notifications;

public sealed record SendPushNotificationNotification(UserRef UserId, string Message) : INotification;

public sealed class SendPushNotificationNotificationHandler(ILogger<SendPushNotificationNotificationHandler> logger) : INotificationHandler<SendPushNotificationNotification>
{
    private readonly ILogger<SendPushNotificationNotificationHandler> _logger = logger.AssertNotNull();

    public async Task HandleAsync(SendPushNotificationNotification notification)
    {
        await Task.Yield();
        
        // Simulate sending push notification
        _logger.LogInformation($"[PUSH] To User: {notification.UserId}, Message: {notification.Message}");
    }
}
