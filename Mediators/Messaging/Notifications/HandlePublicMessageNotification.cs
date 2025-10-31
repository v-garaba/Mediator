using Mediators.Models;
using Mediators.Repository;

namespace Mediators.Messaging.Notifications;

// NOTE: Broken down from SendMessageNotification
public sealed record HandlePublicMessageNotification(
    ChatMessage Message
) : INotification;

public sealed class HandlePublicMessageNotificationHandler(
    IMediator mediator,
    IStorage<UserRef, User> userStorage)
    : INotificationHandler<HandlePublicMessageNotification>
{
    private readonly IMediator _mediator = mediator.AssertNotNull();
    private readonly IStorage<UserRef, User> _userStorage = userStorage.AssertNotNull();

    public async Task HandleAsync(HandlePublicMessageNotification notification)
    {
        if (notification.Message.Type != MessageType.Public)
        {
            throw new InvalidOperationException("Notification type must be Public for HandlePublicMessageNotification");
        }

        var users = await _userStorage.GetAllAsync();
        foreach (var user in users)
        {
            if (user.Id != notification.Message.SenderId)
            {
                await _mediator.PublishAsync(new NotifyUserOfMessageNotification(user, notification.Message));
            }
        }
    }
}
