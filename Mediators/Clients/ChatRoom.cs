using Mediators.Messaging;
using Mediators.Messaging.Notifications;
using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Clients;

public class ChatRoom(IMediator mediator, ILogger<ChatRoom> logger)
{
    private readonly ILogger<ChatRoom> _logger = logger;
    private readonly IMediator _mediator = mediator;

    public async Task SendMessageAsync(
        UserRef senderId,
        string content,
        MessageType type,
        UserRef? targetUserId = null
    )
    {
        _logger.LogInformation(
            $"[CHAT ROOM] User {senderId} is sending a {(type == MessageType.Private ? "private" : "public")} message"
        );
        await _mediator.PublishAsync(new SendMessageNotification(senderId, content, type, targetUserId));
    }

    public async Task AddUserAsync(User user)
    {
        _logger.LogInformation($"[CHAT ROOM] User {user.Name} is joining the chat room");
        await _mediator.PublishAsync(new AddUserNotification(user));
    }

    public async Task ChangeUserStatusAsync(UserRef userId, UserStatus newStatus)
    {
        _logger.LogInformation($"[CHAT ROOM] User {userId} is changing status to {newStatus}");
        await _mediator.PublishAsync(new ChangeUserStatusNotification(userId, newStatus));
    }
}
