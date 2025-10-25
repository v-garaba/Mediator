namespace Mediators.Messaging.Notifications;

public interface INotificationObserver
{
    Task Publish(INotification notification);
    void Subscribe<TNotification>(Func<TNotification, Task> handler)
        where TNotification : INotification;
}
