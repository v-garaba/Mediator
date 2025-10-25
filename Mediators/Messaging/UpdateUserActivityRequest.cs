namespace Mediators.Messaging;

public sealed record UpdateUserActivityRequest(string UserId) : IRequest;
