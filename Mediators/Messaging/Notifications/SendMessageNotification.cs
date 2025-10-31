using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.Logging;

namespace Mediators.Messaging.Notifications;

public sealed record SendMessageNotification(
    UserRef SenderId,
    string Content,
    MessageType Type,
    UserRef? TargetUserId = null
) : INotification;

public sealed class SendMessageNotificationHandler(
    IMediator mediator,
    IStorage<UserRef, User> userStorage,
    IStorage<MessageRef, ChatMessage> messageStorage,
    ILogger<SendMessageNotificationHandler> logger)
    : INotificationHandler<SendMessageNotification>
{
    private readonly IMediator _mediator = mediator.AssertNotNull();
    private readonly IStorage<UserRef, User> _userStorage = userStorage.AssertNotNull();
    private readonly IStorage<MessageRef, ChatMessage> _messageStorage = messageStorage.AssertNotNull();
    private readonly ILogger<SendMessageNotificationHandler> _logger = logger.AssertNotNull();

    public async Task HandleAsync(SendMessageNotification notification)
    {
        var message = new ChatMessage(
            notification.SenderId,
            notification.Content,
            notification.Type,
            notification.TargetUserId
        );

        await _messageStorage.SetAsync(message);

        _logger.LogInformation($"Message sent by {notification.SenderId}: {notification.Content}");
        await _mediator.PublishAsync(new StoreMessageNotification(message));
        await _mediator.PublishAsync(
            new TrackMessageSentNotification(notification.SenderId, notification.Type.ToString())
        );

        switch (notification.Type)
        {
            case MessageType.Private:
                await _mediator.PublishAsync(new HandlePrivateMessageNotification(message));
                break;
            case MessageType.Public:
                await _mediator.PublishAsync(new HandlePublicMessageNotification(message));
                break;
            case MessageType.System:
                break;
        }

        await _mediator.PublishAsync(new UpdateUserActivityNotification(notification.SenderId));
    }
}
