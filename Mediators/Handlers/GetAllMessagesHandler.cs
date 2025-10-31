using Mediators.Models;
using Mediators.Repository;

namespace Mediators.Handlers;

public sealed record GetAllMessagesRequest() : IRequest<GetAllMessagesResponse>;

public sealed record GetAllMessagesResponse(IReadOnlyList<ChatMessage> Messages);

public sealed class GetAllMessagesHandler(IStorage<MessageRef, ChatMessage> messageStorage)
    : IRequestHandler<GetAllMessagesRequest, GetAllMessagesResponse>
{
    private readonly IStorage<MessageRef, ChatMessage> _messageStorage = messageStorage;

    public async Task<GetAllMessagesResponse> HandleAsync(GetAllMessagesRequest _, CancellationToken cancellationToken)
    {
        var messages = await _messageStorage.GetAllAsync(cancellationToken);
        return new GetAllMessagesResponse(messages);
    }
}
