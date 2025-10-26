using Mediators.Models;

namespace Mediators.Messaging.Requests;

public sealed record GetMessagesByUserRequest(string UserId) : IRequest<GetMessagesByUserResponse>;

public sealed record GetMessagesByUserResponse(IReadOnlyList<ChatMessage> Messages);
