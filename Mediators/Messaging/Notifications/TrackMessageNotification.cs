namespace Mediators.Messaging.Notifications;

public sealed record TrackMessageNotification(string UserId, string MessageId) : INotification;
