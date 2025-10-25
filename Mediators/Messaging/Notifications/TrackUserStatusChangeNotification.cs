namespace Mediators.Messaging.Notifications;

public sealed record TrackUserStatusChangeNotification(string UserId, string Status)
    : INotification;
