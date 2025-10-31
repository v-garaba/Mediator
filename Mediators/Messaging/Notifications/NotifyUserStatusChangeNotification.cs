using Mediators.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mediators.Messaging.Notifications;

public sealed record NotifyUserStatusChangeNotification(
    User User,
    UserStatus OldStatus,
    UserStatus NewStatus
) : INotification;

public sealed class NotifyUserStatusChangeNotificationHandler(
    IServiceProvider serviceProvider,
    ILogger<NotifyUserStatusChangeNotificationHandler> logger)
    : INotificationHandler<NotifyUserStatusChangeNotification>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider.AssertNotNull();
    private readonly ILogger<NotifyUserStatusChangeNotificationHandler> _logger = logger.AssertNotNull();

    public async Task HandleAsync(NotifyUserStatusChangeNotification notification)
    {
        _logger.LogInformation(
            $"User {notification.User.Name} status changed from {notification.OldStatus} to {notification.NewStatus}"
        );

        if (notification.NewStatus == UserStatus.Online)
        {
            // Lazy resolve mediator to avoid circular dependency
            var mediator = _serviceProvider.GetRequiredService<IMediator>();
            
            await mediator.PublishAsync(
                new SendPushNotificationNotification(notification.User.Id, "You are now online")
            );
            await mediator.PublishAsync(
                new TrackUserStatusChangeNotification(notification.User.Id, notification.NewStatus)
            );
        }
    }
}
