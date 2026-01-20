using Microsoft.Extensions.Logging;

namespace Mediators.Notifications;

public sealed class EmailNotificationHandler(ILogger<EmailNotificationHandler> logger) : AbstractNotificationHandler<EmailNotification>
{
    private readonly ILogger<EmailNotificationHandler> _logger = logger.AssertNotNull();

    public sealed override async Task HandleAsync(EmailNotification notification)
    {
        await Task.Yield();
        
        // Simulate sending email
        _logger.LogInformation(
            $"[EMAIL] To: {notification.To}, Subject: {notification.Subject}, Body: {notification.Body}"
        );
    }
}


