using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Messaging.Notifications;

public sealed record SendSmsNotification(UserRef UserId, string Message) : INotification;

public sealed class SendSmsNotificationHandler(ILogger<SendSmsNotificationHandler> logger) : INotificationHandler<SendSmsNotification>
{
    private readonly ILogger<SendSmsNotificationHandler> _logger = logger.AssertNotNull();

    public async Task HandleAsync(SendSmsNotification notification)
    {
        await Task.Yield();
        
        // Simulate sending SMS
        _logger.LogInformation($"[SMS] To User: {notification.UserId}, Message: {notification.Message}");
    }
}
