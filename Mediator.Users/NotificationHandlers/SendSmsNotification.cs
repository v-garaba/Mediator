using Microsoft.Extensions.Logging;

namespace Mediators.Notifications;

public sealed class SendSmsNotificationHandler(ILogger<SendSmsNotificationHandler> logger)
    : AbstractNotificationHandler<SendSmsNotification>
{
    private readonly ILogger<SendSmsNotificationHandler> _logger = logger.AssertNotNull();

    public sealed override async Task HandleAsync(SendSmsNotification notification)
    {
        await Task.Yield();

        // Simulate sending SMS
        _logger.LogInformation(
            $"[SMS] To User: {notification.UserId}, Message: {notification.Message}"
        );
    }
}


