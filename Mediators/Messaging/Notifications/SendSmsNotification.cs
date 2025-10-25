namespace Mediators.Messaging.Notifications;

public sealed record SendSmsNotification(string UserId, string Message) : INotification;
