using Mediators.Mediators;
using Mediators.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mediators.Notifications;

public sealed class NotifyUserOfMessageNotificationHandler(
    IServiceProvider serviceProvider,
    ILogger<NotifyUserOfMessageNotificationHandler> logger)
    : AbstractNotificationHandler<NotifyUserOfMessageNotification>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider.AssertNotNull();
    private readonly ILogger<NotifyUserOfMessageNotificationHandler> _logger = logger.AssertNotNull();

    public sealed override async Task HandleAsync(NotifyUserOfMessageNotification notification)
    {
        _logger.LogInformation($"Notifying user {notification.User.Name} of new message");

        // Lazy resolve mediator to avoid circular dependency
        var mediator = _serviceProvider.GetRequiredService<IMediator>();

        // User may go offline while this is being processed
        await mediator.PublishAsync(
            new EmailNotification(notification.User.Email, "New Message", notification.Message.Content));
        await mediator.PublishAsync(
            new SendSmsNotification(notification.User.Id, $"New message from {notification.Message.SenderId}"));

        // Direct signal the user when they are not offline
        if (notification.User.Status != UserStatus.Offline)
        {
            await mediator.PublishAsync(
               new SendPushNotificationNotification(notification.User.Id, notification.Message.Content));
        }

        await mediator.PublishAsync(
            new TrackMessageNotification(notification.User.Id, notification.Message.Id));
    }
}


