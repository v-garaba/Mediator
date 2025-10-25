using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class MessageStorageService
{
    private readonly ILogger<MessageStorageService> _logger;
    private readonly List<ChatMessage> _storage = new();

    public MessageStorageService(ILogger<MessageStorageService> logger)
    {
        _logger = logger;
    }

    public void StoreMessage(ChatMessage message)
    {
        _storage.Add(message);
        _logger.LogInformation($"[STORAGE] Message {message.Id} stored");
    }

    public IReadOnlyList<ChatMessage> GetAllMessages() => _storage.AsReadOnly();

    public IReadOnlyList<ChatMessage> GetMessagesByUser(string userId)
    {
        return _storage.Where(m => m.SenderId == userId).ToList().AsReadOnly();
    }
}
