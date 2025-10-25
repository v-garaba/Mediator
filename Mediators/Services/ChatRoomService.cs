using System.Threading.Tasks;
using Mediators.Messaging;
using Mediators.Messaging.Notifications;
using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class ChatRoomService
{
    private readonly ILogger<ChatRoomService> _logger;
    private readonly ChatMediator _mediator;

    private readonly Dictionary<string, User> _users = [];
    private readonly List<ChatMessage> _messages = [];

    public ChatRoomService(ChatMediator mediator, ILogger<ChatRoomService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    // BAD: This method orchestrates too many things
    public async Task SendMessageAsync(
        string senderId,
        string content,
        MessageType type,
        string? targetUserId = null
    )
    {
        var message = new ChatMessage(senderId, content, type, targetUserId);
        _messages.Add(message);

        _logger.LogInformation($"Message sent by {senderId}: {content}");
        await _mediator.Publish(new StoreMessageNotification(message));
        await _mediator.Publish(new TrackMessageSentNotification(senderId, type.ToString()));

        if (type == MessageType.Private && targetUserId != null)
        {
            if (_users.TryGetValue(targetUserId, out var targetUser))
            {
                await _mediator.Publish(new NotifyUserOfMessageNotification(targetUser, message));
            }
        }
        else if (type == MessageType.Public)
        {
            foreach (var user in _users.Values)
            {
                if (user.Id != senderId)
                {
                    await _mediator.Publish(new NotifyUserOfMessageNotification(user, message));
                }
            }
        }

        await _mediator.Publish(new UpdateUserActivityNotification(senderId));
    }

    public async Task AddUserAsync(User user)
    {
        _users[user.Id] = user;
        _logger.LogInformation($"User {user.Name} joined the chat room");
        await _mediator.Publish(new RegisterUserNotification(user));

        var systemMessage = new ChatMessage(
            "SYSTEM",
            $"{user.Name} joined the chat",
            MessageType.System
        );
        _messages.Add(systemMessage);
        await _mediator.Publish(new StoreMessageNotification(systemMessage));
    }

    public async Task ChangeUserStatusAsync(string userId, UserStatus newStatus)
    {
        if (_users.TryGetValue(userId, out var user))
        {
            var oldStatus = user.Status;
            user = user with { Status = newStatus };

            _logger.LogInformation($"User {user.Name} status changed to {newStatus}");
            await _mediator.Publish(
                new NotifyUserStatusChangeNotification(user, oldStatus, newStatus)
            );
            await _mediator.Publish(new UpdateUserStatusNotification(userId, newStatus));
        }
    }
}
