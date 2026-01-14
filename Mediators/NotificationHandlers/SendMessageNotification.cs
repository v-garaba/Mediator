using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mediators.NotificationHandlers;

public sealed record SendMessageNotification(
    UserRef SenderId,
    string Content,
    MessageType Type,
    UserRef? TargetUserId = null
) : INotification;

public sealed class SendMessageNotificationHandler(
    IServiceProvider serviceProvider,
    IStorage<MessageRef, ChatMessage> messageStorage,
    ILogger<SendMessageNotificationHandler> logger
) : AbstractNotificationHandler<SendMessageNotification>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider.AssertNotNull();
    private readonly IStorage<MessageRef, ChatMessage> _messageStorage =
        messageStorage.AssertNotNull();
    private readonly ILogger<SendMessageNotificationHandler> _logger = logger.AssertNotNull();

    public sealed override async Task HandleAsync(SendMessageNotification notification)
    {
        var message = new ChatMessage(
            notification.SenderId,
            notification.Content,
            notification.Type,
            notification.TargetUserId
        );

        await _messageStorage.SetAsync(message);

        _logger.LogInformation($"Message sent by {notification.SenderId}: {notification.Content}");

        // Lazy resolve mediator to avoid circular dependency
        var mediator = _serviceProvider.GetRequiredService<IMediator>();

        await mediator.PublishAsync(new StoreMessageNotification(message));
        await mediator.PublishAsync(
            new TrackMessageSentNotification(notification.SenderId, notification.Type.ToString())
        );

        switch (notification.Type)
        {
            case MessageType.Private:
                await mediator.PublishAsync(new HandlePrivateMessageNotification(message));
                break;
            case MessageType.Public:
                await mediator.PublishAsync(new HandlePublicMessageNotification(message));
                break;
            case MessageType.System:
                break;
        }

        await mediator.PublishAsync(new UpdateUserActivityNotification(notification.SenderId));
    }
}
