using Mediators.Models;
using Mediators.Repository;
using System.Collections.Immutable;

namespace Mediators.RequestHandlers;

public sealed record GetMessagesByUserRequest(UserRef UserId) : IRequest<GetMessagesByUserResponse>;

public sealed record GetMessagesByUserResponse(ImmutableArray<ChatMessage> Messages);

public sealed class GetMessagesByUserHandler(IStorage<MessageRef, ChatMessage> messageStorage)
    : IRequestHandler<GetMessagesByUserRequest, GetMessagesByUserResponse>
{
    private readonly IStorage<MessageRef, ChatMessage> _messageStorage = messageStorage;

    public async Task<GetMessagesByUserResponse> HandleAsync(GetMessagesByUserRequest request, CancellationToken cancellationToken)
    {
        var allMessages = await _messageStorage.GetAllAsync(cancellationToken);
        var userMessages = allMessages.Where(msg => msg.SenderId == request.UserId).ToImmutableArray();
        return new GetMessagesByUserResponse(userMessages);
    }
}
