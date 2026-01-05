using Mediators.Models;
using Mediators.Repository;

namespace Mediators.RequestHandlers;

public sealed record GetMessageCountRequest(UserRef UserId) : IRequest<GetMessageCountResponse>;

public sealed record GetMessageCountResponse(int Count);

public sealed class GetMessageCountHandler(IStorage<MessageRef, ChatMessage> messageStorage)
    : AbstractRequestHandler<GetMessageCountRequest, GetMessageCountResponse>
{
    private readonly IStorage<MessageRef, ChatMessage> _messageStorage = messageStorage;

    public sealed override async Task<GetMessageCountResponse> HandleAsync(GetMessageCountRequest request, CancellationToken cancellationToken)
    {
        var userMessageCount = await _messageStorage.CountAsync(cancellationToken);
        return new GetMessageCountResponse(userMessageCount);
    }
}