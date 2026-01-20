using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Mediators.Notifications;

public sealed class HandlePrivateMessageNotificationHandler(
    IServiceProvider serviceProvider,
    IStorage<UserRef, User> userStorage)
    : AbstractNotificationHandler<HandlePrivateMessageNotification>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider.AssertNotNull();
    private readonly IStorage<UserRef, User> _userStorage = userStorage.AssertNotNull();

    public sealed override async Task HandleAsync(HandlePrivateMessageNotification notification)
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
            // Lazy resolve mediator to avoid circular dependency
            var mediator = _serviceProvider.GetRequiredService<IMediator>();
            await mediator.PublishAsync(new NotifyUserOfMessageNotification(targetUser, notification.Message));
        }
    }
}


