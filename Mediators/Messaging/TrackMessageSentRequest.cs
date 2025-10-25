namespace Mediators.Messaging;

public sealed record TrackMessageSentRequest(string UserId, string MessageType) : IRequest;
