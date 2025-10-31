using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Messaging.Notifications;

public sealed record NotifyUserOfMessageNotification(User User, ChatMessage Message)
    : INotification;

public sealed class NotifyUserOfMessageNotificationHandler(
    IMediator mediator,
    ILogger<NotifyUserOfMessageNotificationHandler> logger)
    : INotificationHandler<NotifyUserOfMessageNotification>
{
    private readonly IMediator _mediator = mediator.AssertNotNull();
    private readonly ILogger<NotifyUserOfMessageNotificationHandler> _logger = logger.AssertNotNull();

    public async Task HandleAsync(NotifyUserOfMessageNotification notification)
    {
        _logger.LogInformation($"Notifying user {notification.User.Name} of new message");

        if (notification.User.Status == UserStatus.Offline)
        {
            await _mediator.PublishAsync(
                new EmailNotification(notification.User.Email, "New Message", notification.Message.Content)
            );
            await _mediator.PublishAsync(
                new SendSmsNotification(notification.User.Id, $"New message from {notification.Message.SenderId}")
            );
        }
        else
        {
            await _mediator.PublishAsync(
                new SendPushNotificationNotification(notification.User.Id, notification.Message.Content)
            );
        }

        await _mediator.PublishAsync(
            new TrackMessageNotification(notification.User.Id, notification.Message.Id)
        );
    }
}
