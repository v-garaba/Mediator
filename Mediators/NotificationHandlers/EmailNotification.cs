using Microsoft.Extensions.Logging;

namespace Mediators.NotificationHandlers;

/// <summary>
/// Marker interface for messages
/// </summary>
public sealed record EmailNotification(string To, string Subject, string Body) : INotification;

public sealed class EmailNotificationHandler(ILogger<EmailNotificationHandler> logger) : INotificationHandler<EmailNotification>
{
    private readonly ILogger<EmailNotificationHandler> _logger = logger.AssertNotNull();

    public async Task HandleAsync(EmailNotification notification)
    {
        await Task.Yield();
        
        // Simulate sending email
        _logger.LogInformation(
            $"[EMAIL] To: {notification.To}, Subject: {notification.Subject}, Body: {notification.Body}"
        );
    }
}
