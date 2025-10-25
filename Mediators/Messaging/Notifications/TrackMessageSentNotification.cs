namespace Mediators.Messaging.Notifications;

public sealed record TrackMessageSentNotification(string UserId, string MessageType)
    : INotification;
