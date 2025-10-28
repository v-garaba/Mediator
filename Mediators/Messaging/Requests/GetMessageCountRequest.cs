using Mediators.Models;

namespace Mediators.Messaging.Requests;

public sealed record GetMessageCountRequest(UserRef UserId) : IRequest<GetMessageCountResponse>;

public sealed record GetMessageCountResponse(int Count);
