using Mediators.Messaging;
using Mediators.Messaging.Notifications;
using Mediators.Messaging.Requests;
using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class MessageStorageService
{
    private readonly ILogger<MessageStorageService> _logger;
    private readonly ChatMediator _mediator;
    private readonly List<ChatMessage> _storage = [];

    public MessageStorageService(ChatMediator mediator, ILogger<MessageStorageService> logger)
    {
        _logger = logger;
        _mediator = mediator;

        _mediator.Subscribe<StoreMessageNotification>(StoreMessageAsync);

        _mediator.RegisterHandler<GetAllMessagesRequest, GetAllMessagesResponse>(GetAllMessages);
        _mediator.RegisterHandler<GetMessagesByUserRequest, GetMessagesByUserResponse>(
            GetMessagesByUser
        );
    }

    private async Task StoreMessageAsync(StoreMessageNotification message)
    {
        await Task.Yield();
        _storage.Add(message.Message);
        _logger.LogInformation($"[STORAGE] Message {message.Message.Id} stored");
    }

    private async Task<GetAllMessagesResponse> GetAllMessages(GetAllMessagesRequest _)
    {
        await Task.Yield();
        return new GetAllMessagesResponse(_storage.AsReadOnly());
    }

    private async Task<GetMessagesByUserResponse> GetMessagesByUser(GetMessagesByUserRequest request)
    {
        await Task.Yield();
        return new GetMessagesByUserResponse(
            _storage.Where(m => m.SenderId == request.UserId).ToList().AsReadOnly()
        );
    }
}
