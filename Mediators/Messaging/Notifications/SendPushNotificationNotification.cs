namespace Mediators.Messaging.Notifications;

public sealed record SendPushNotificationNotification(string UserId, string Message)
    : INotification;
