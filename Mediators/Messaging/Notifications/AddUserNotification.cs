using Mediators.Data;
using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mediators.Messaging.Notifications;

public sealed record AddUserNotification(User User) : INotification;

public sealed class AddUserNotificationHandler(
    ILogger<AddUserNotificationHandler> logger,
    IServiceProvider serviceProvider,
    IStorage<UserRef, User> userStorage,
    IStorage<MessageRef, ChatMessage> messageStorage)
    : INotificationHandler<AddUserNotification>
{
    private readonly ILogger<AddUserNotificationHandler> _logger = logger.AssertNotNull();
    private readonly IServiceProvider _serviceProvider = serviceProvider.AssertNotNull();
    private readonly IStorage<UserRef, User> _userStorage = userStorage.AssertNotNull();
    private readonly IStorage<MessageRef, ChatMessage> _messageStorage = messageStorage.AssertNotNull();

    public async Task HandleAsync(AddUserNotification notification)
    {
        await _userStorage.SetAsync(notification.User).ConfigureAwait(false);
        _logger.LogInformation($"User {notification.User.Name} joined the chat room");

        // Lazy resolve mediator to avoid circular dependency
        var mediator = _serviceProvider.GetRequiredService<IMediator>();

        await mediator.PublishAsync(new RegisterUserNotification(notification.User));

        var systemMessage = new ChatMessage(
            ChatSystem.Reference,
            $"{notification.User.Name} joined the chat",
            MessageType.System
        );

        await _messageStorage.SetAsync(systemMessage);
        await mediator.PublishAsync(new StoreMessageNotification(systemMessage));
    }
}
