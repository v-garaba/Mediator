namespace Mediators.Messaging.Requests;

public sealed record GetMessageCountRequest(string UserId) : IRequest<GetMessageCountResponse>;

public sealed record GetMessageCountResponse(int Count);
