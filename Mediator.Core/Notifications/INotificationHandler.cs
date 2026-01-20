namespace Mediators.Notifications;

public interface INotificationHandler
{
    Type NotificationType { get; }
}

public interface INotificationHandler<TNotification> : INotificationHandler
    where TNotification : INotification
{
    Task HandleAsync(TNotification notification);
}


