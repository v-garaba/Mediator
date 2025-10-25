using Mediators.Messaging;
using Mediators.Messaging.Notifications;
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
    }

    private async Task StoreMessageAsync(StoreMessageNotification message)
    {
        await Task.Yield();
        _storage.Add(message.Message);
        _logger.LogInformation($"[STORAGE] Message {message.Message.Id} stored");
    }

    public IReadOnlyList<ChatMessage> GetAllMessages() => _storage.AsReadOnly();

    public IReadOnlyList<ChatMessage> GetMessagesByUser(string userId)
    {
        return _storage.Where(m => m.SenderId == userId).ToList().AsReadOnly();
    }
}
