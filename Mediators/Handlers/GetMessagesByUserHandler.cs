using Mediators.Models;
using Mediators.Repository;

namespace Mediators.Handlers;

public sealed record GetMessagesByUserRequest(UserRef UserId) : IRequest<GetMessagesByUserResponse>;

public sealed record GetMessagesByUserResponse(IReadOnlyList<ChatMessage> Messages);

public sealed class GetMessagesByUserHandler(IStorage<MessageRef, ChatMessage> messageStorage)
    : IRequestHandler<GetMessagesByUserRequest, GetMessagesByUserResponse>
{
    private readonly IStorage<MessageRef, ChatMessage> _messageStorage = messageStorage;

    public async Task<GetMessagesByUserResponse> HandleAsync(GetMessagesByUserRequest request, CancellationToken cancellationToken)
    {
        var allMessages = await _messageStorage.GetAllAsync(cancellationToken);
        var userMessages = allMessages.Where(msg => msg.SenderId == request.UserId).ToList();
        return new GetMessagesByUserResponse(userMessages);
    }
}
