namespace Mediators.NotificationHandlers;

public interface INotificationHandler { }
public interface INotificationHandler<TNotification> : INotificationHandler
    where TNotification : INotification
{
    Task HandleAsync(TNotification notification);
}
