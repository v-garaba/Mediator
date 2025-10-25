namespace Mediators.Messaging;

public sealed record TrackUserStatusChangeRequest(string UserId, string Status) : IRequest;
