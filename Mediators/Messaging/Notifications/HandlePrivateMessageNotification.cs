using Mediators.Models;
using Mediators.Repository;

namespace Mediators.Messaging.Notifications;

// NOTE: Broken down from SendMessageNotification
public sealed record HandlePrivateMessageNotification(
    ChatMessage Message
) : INotification;

public sealed class HandlePrivateMessageNotificationHandler(
    IMediator mediator,
    IStorage<UserRef, User> userStorage)
    : INotificationHandler<HandlePrivateMessageNotification>
{
    private readonly IMediator _mediator = mediator.AssertNotNull();
    private readonly IStorage<UserRef, User> _userStorage = userStorage.AssertNotNull();

    public async Task HandleAsync(HandlePrivateMessageNotification notification)
    {
        if (notification.Message.Type != MessageType.Private)
        {
            throw new InvalidOperationException("Notification type must be Private for HandlePrivateMessageNotification");
        }

        if (notification.Message.TargetUserId == null)
        {
            throw new InvalidOperationException("TargetUserId must be set for Private messages");
        }

        var targetUser = await _userStorage.TryGetAsync(notification.Message.TargetUserId);
        if (targetUser != null)
        {
            await _mediator.PublishAsync(new NotifyUserOfMessageNotification(targetUser, notification.Message));
        }
    }
}
