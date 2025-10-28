using Mediators.Messaging;
using Mediators.Messaging.Notifications;
using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Clients;

public class ChatRoom(ChatMediator mediator, ILogger<ChatRoom> logger)
{
    private readonly ILogger<ChatRoom> _logger = logger;
    private readonly ChatMediator _mediator = mediator;

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
        await _mediator.Publish(new SendMessageNotification(senderId, content, type, targetUserId));
    }

    public async Task AddUserAsync(User user)
    {
        _logger.LogInformation($"[CHAT ROOM] User {user.Name} is joining the chat room");
        await _mediator.Publish(new AddUserNotification(user));
    }

    public async Task ChangeUserStatusAsync(UserRef userId, UserStatus newStatus)
    {
        _logger.LogInformation($"[CHAT ROOM] User {userId} is changing status to {newStatus}");
        await _mediator.Publish(new ChangeUserStatusNotification(userId, newStatus));
    }
}
