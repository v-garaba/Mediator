namespace Mediators.Messaging.Notifications;

public sealed record UpdateUserActivityNotification(string UserId) : INotification;
