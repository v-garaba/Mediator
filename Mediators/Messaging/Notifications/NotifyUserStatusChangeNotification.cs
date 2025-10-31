using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Messaging.Notifications;

public sealed record NotifyUserStatusChangeNotification(
    User User,
    UserStatus OldStatus,
    UserStatus NewStatus
) : INotification;

public sealed class NotifyUserStatusChangeNotificationHandler(
    IMediator mediator,
    ILogger<NotifyUserStatusChangeNotificationHandler> logger)
    : INotificationHandler<NotifyUserStatusChangeNotification>
{
    private readonly IMediator _mediator = mediator.AssertNotNull();
    private readonly ILogger<NotifyUserStatusChangeNotificationHandler> _logger = logger.AssertNotNull();

    public async Task HandleAsync(NotifyUserStatusChangeNotification notification)
    {
        _logger.LogInformation(
            $"User {notification.User.Name} status changed from {notification.OldStatus} to {notification.NewStatus}"
        );

        if (notification.NewStatus == UserStatus.Online)
        {
            await _mediator.PublishAsync(
                new SendPushNotificationNotification(notification.User.Id, "You are now online")
            );
            await _mediator.PublishAsync(
                new TrackUserStatusChangeNotification(notification.User.Id, notification.NewStatus)
            );
        }
    }
}
