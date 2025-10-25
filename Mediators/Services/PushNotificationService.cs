using Mediators.Messaging;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class PushNotificationService
{
    private readonly ILogger<PushNotificationService> _logger;
    private readonly MessageBus _messageBus;

    public PushNotificationService(MessageBus messageBus, ILogger<PushNotificationService> logger)
    {
        _logger = logger;
        _messageBus = messageBus;

        _messageBus.Subscribe<SendPushNotificationRequest>(SendPushNotificationAsync);
    }

    private async Task SendPushNotificationAsync(SendPushNotificationRequest message)
    {
        await Task.Yield();
        // Simulate sending push notification
        _logger.LogInformation($"[PUSH] To User: {message.UserId}, Message: {message.Message}");
    }
}
