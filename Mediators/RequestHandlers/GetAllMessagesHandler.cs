using Mediators.Models;
using Mediators.Repository;
using System.Collections.Immutable;

namespace Mediators.RequestHandlers;

public sealed record GetAllMessagesRequest() : IRequest<GetAllMessagesResponse>;

public sealed record GetAllMessagesResponse(ImmutableArray<ChatMessage> Messages);

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
