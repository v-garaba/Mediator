using Mediators.Messaging;
using Mediators.Messaging.Notifications;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class PushNotificationService
{
    private readonly ILogger<PushNotificationService> _logger;
    private readonly ChatMediator _mediator;

    public PushNotificationService(ChatMediator mediator, ILogger<PushNotificationService> logger)
    {
        _logger = logger;
        _mediator = mediator;

        _mediator.Subscribe<SendPushNotificationNotification>(SendPushNotificationAsync);
    }

    private async Task SendPushNotificationAsync(SendPushNotificationNotification message)
    {
        await Task.Yield();
        // Simulate sending push notification
        _logger.LogInformation($"[PUSH] To User: {message.UserId}, Message: {message.Message}");
    }
}
