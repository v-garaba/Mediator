namespace Mediators.Messaging;

public sealed record TrackMessageNotificationRequest(string UserId, string MessageId) : IRequest;
