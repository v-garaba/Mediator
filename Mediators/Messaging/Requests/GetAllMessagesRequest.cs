using Mediators.Models;

namespace Mediators.Messaging.Requests;

public sealed record GetAllMessagesRequest(string UserId) : IRequest<GetAllMessagesResponse>;

public sealed record GetAllMessagesResponse(IReadOnlyList<ChatMessage> Messages);
