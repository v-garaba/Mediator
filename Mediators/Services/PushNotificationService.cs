using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class PushNotificationService
{
    private readonly ILogger<PushNotificationService> _logger;

    public PushNotificationService(ILogger<PushNotificationService> logger)
    {
        _logger = logger;
    }

    public void SendPushNotification(string userId, string message)
    {
        // Simulate sending push notification
        _logger.LogInformation($"[PUSH] To User: {userId}, Message: {message}");
    }
}
