using Mediators.Messaging;
using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class ChatRoomService
{
    private readonly ILogger<ChatRoomService> _logger;
    private readonly MessageBus _messageBus;

    private readonly Dictionary<string, User> _users = [];
    private readonly List<ChatMessage> _messages = [];

    public ChatRoomService(MessageBus messageBus, ILogger<ChatRoomService> logger)
    {
        _messageBus = messageBus;
        _logger = logger;
    }

    // BAD: This method orchestrates too many things
    public void SendMessage(
        string senderId,
        string content,
        MessageType type,
        string? targetUserId = null
    )
    {
        var message = new ChatMessage(senderId, content, type, targetUserId);
        _messages.Add(message);

        _logger.LogInformation($"Message sent by {senderId}: {content}");
        _messageBus.Publish(new StoreMessageRequest(message));
        _messageBus.Publish(new TrackMessageSentRequest(senderId, type.ToString()));

        if (type == MessageType.Private && targetUserId != null)
        {
            if (_users.TryGetValue(targetUserId, out var targetUser))
            {
                _messageBus.Publish(new NotifyUserOfMessageRequest(targetUser, message));
            }
        }
        else if (type == MessageType.Public)
        {
            foreach (var user in _users.Values)
            {
                if (user.Id != senderId)
                {
                    _messageBus.Publish(new NotifyUserOfMessageRequest(user, message));
                }
            }
        }

        _messageBus.Publish(new UpdateUserActivityRequest(senderId));
    }

    public void AddUser(User user)
    {
        _users[user.Id] = user;
        _logger.LogInformation($"User {user.Name} joined the chat room");
        _messageBus.Publish(new RegisterUserRequest(user));

        var systemMessage = new ChatMessage(
            "SYSTEM",
            $"{user.Name} joined the chat",
            MessageType.System
        );
        _messages.Add(systemMessage);
        _messageBus.Publish(new StoreMessageRequest(systemMessage));
    }

    public void ChangeUserStatus(string userId, UserStatus newStatus)
    {
        if (_users.TryGetValue(userId, out var user))
        {
            var oldStatus = user.Status;
            user = user with { Status = newStatus };

            _logger.LogInformation($"User {user.Name} status changed to {newStatus}");
            _messageBus.Publish(new NotifyUserStatusChangeRequest(user, oldStatus, newStatus));
            _messageBus.Publish(new UpdateUserStatusRequest(userId, newStatus));
        }
    }
}
