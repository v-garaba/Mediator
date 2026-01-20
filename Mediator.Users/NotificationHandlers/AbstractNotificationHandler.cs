namespace Mediators.NotificationHandlers;

public abstract class AbstractNotificationHandler<TNotification> : INotificationHandler<TNotification>
    where TNotification : INotification
{
    public Type NotificationType { get; } = typeof(TNotification);

    public abstract Task HandleAsync(TNotification notification);
}
