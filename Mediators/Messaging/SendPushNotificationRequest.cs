namespace Mediators.Messaging;

public sealed record SendPushNotificationRequest(string UserId, string Message) : IRequest;
