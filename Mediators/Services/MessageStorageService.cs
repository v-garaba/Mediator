using Mediators.Messaging;
using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class MessageStorageService
{
    private readonly ILogger<MessageStorageService> _logger;
    private readonly MessageBus _messageBus;
    private readonly List<ChatMessage> _storage = [];

    public MessageStorageService(MessageBus messageBus, ILogger<MessageStorageService> logger)
    {
        _logger = logger;
        _messageBus = messageBus;

        _messageBus.Subscribe<StoreMessageRequest>(StoreMessageAsync);
    }

    private async Task StoreMessageAsync(StoreMessageRequest message)
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
