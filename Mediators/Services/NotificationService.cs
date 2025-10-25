using Mediators.Messaging;
using Mediators.Messaging.Notifications;
using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

/// <summary>
/// BADLY DESIGNED: This service is tightly coupled with many other components
/// </summary>
public class NotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly ChatMediator _mediator;

    public NotificationService(ChatMediator mediator, ILogger<NotificationService> logger)
    {
        _logger = logger;
        _mediator = mediator;

        _mediator.Subscribe<NotifyUserOfMessageNotification>(NotifyUserOfMessageAsync);
        _mediator.Subscribe<NotifyUserStatusChangeNotification>(NotifyUserStatusChangeAsync);
    }

    private async Task NotifyUserOfMessageAsync(NotifyUserOfMessageNotification Notification)
    {
        _logger.LogInformation($"Notifying user {Notification.User.Name} of new message");

        // BAD: Tight coupling - directly calling multiple services
        if (Notification.User.Status == UserStatus.Offline)
        {
            await _mediator.Publish(
                new EmailNotification(Notification.User.Email, "New Message", Notification.Message.Content)
            );
            await _mediator.Publish(
                new SendSmsNotification(Notification.User.Id, $"New message from {Notification.Message.SenderId}")
            );
        }
        else
        {
            await _mediator.Publish(
                new SendPushNotificationNotification(Notification.User.Id, Notification.Message.Content)
            );
        }

        await _mediator.Publish(
            new TrackMessageNotification(Notification.User.Id, Notification.Message.Id)
        );
    }

    private async Task NotifyUserStatusChangeAsync(NotifyUserStatusChangeNotification Notification)
    {
        _logger.LogInformation(
            $"User {Notification.User.Name} status changed from {Notification.OldStatus} to {Notification.NewStatus}"
        );

        if (Notification.NewStatus == UserStatus.Online)
        {
            await _mediator.Publish(
                new SendPushNotificationNotification(Notification.User.Id, "You are now online")
            );
            await _mediator.Publish(
                new TrackUserStatusChangeNotification(Notification.User.Id, Notification.NewStatus.ToString())
            );
        }
    }
}
