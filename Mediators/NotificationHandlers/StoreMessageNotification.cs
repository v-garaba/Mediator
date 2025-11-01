using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.Logging;

namespace Mediators.NotificationHandlers;

public sealed record StoreMessageNotification(ChatMessage Message) : INotification;

public sealed class StoreMessageNotificationHandler(
    IStorage<MessageRef, ChatMessage> messageStorage,
    ILogger<StoreMessageNotificationHandler> logger
) : INotificationHandler<StoreMessageNotification>
{
    private readonly IStorage<MessageRef, ChatMessage> _messageStorage = messageStorage;
    private readonly ILogger<StoreMessageNotificationHandler> _logger = logger;

    public async Task HandleAsync(StoreMessageNotification notification)
    {
        await _messageStorage.SetAsync(notification.Message).ConfigureAwait(false);
        _logger.LogInformation($"[STORAGE] Message {notification.Message.Id} stored");
    }
}
