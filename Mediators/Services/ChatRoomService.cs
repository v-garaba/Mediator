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

        _mediator.Subscribe<SendMessageNotification>(SendMessageAsync);
        _mediator.Subscribe<AddUserNotification>(AddUserAsync);
        _mediator.Subscribe<ChangeUserStatusNotification>(ChangeUserStatusAsync);
    }

    private async Task SendMessageAsync(SendMessageNotification notification)
    {
        var message = new ChatMessage(
            notification.SenderId,
            notification.Content,
            notification.Type,
            notification.TargetUserId
        );
        _messages.Add(message);

        _logger.LogInformation($"Message sent by {notification.SenderId}: {notification.Content}");
        await _mediator.Publish(new StoreMessageNotification(message));
        await _mediator.Publish(
            new TrackMessageSentNotification(notification.SenderId, notification.Type.ToString())
        );

        if (notification.Type == MessageType.Private && notification.TargetUserId != null)
        {
            if (_users.TryGetValue(notification.TargetUserId, out var targetUser))
            {
                await _mediator.Publish(new NotifyUserOfMessageNotification(targetUser, message));
            }
        }
        else if (notification.Type == MessageType.Public)
        {
            foreach (var user in _users.Values)
            {
                if (user.Id != notification.SenderId)
                {
                    await _mediator.Publish(new NotifyUserOfMessageNotification(user, message));
                }
            }
        }

        await _mediator.Publish(new UpdateUserActivityNotification(notification.SenderId));
    }

    private async Task AddUserAsync(AddUserNotification notification)
    {
        _users[notification.User.Id] = notification.User;
        _logger.LogInformation($"User {notification.User.Name} joined the chat room");
        await _mediator.Publish(new RegisterUserNotification(notification.User));

        var systemMessage = new ChatMessage(
            "SYSTEM",
            $"{notification.User.Name} joined the chat",
            MessageType.System
        );
        _messages.Add(systemMessage);
        await _mediator.Publish(new StoreMessageNotification(systemMessage));
    }

    private async Task ChangeUserStatusAsync(ChangeUserStatusNotification notification)
    {
        if (_users.TryGetValue(notification.UserId, out var user))
        {
            var oldStatus = user.Status;
            user = user with { Status = notification.NewStatus };

            _logger.LogInformation($"User {user.Name} status changed to {notification.NewStatus}");
            await _mediator.Publish(
                new NotifyUserStatusChangeNotification(user, oldStatus, notification.NewStatus)
            );
            await _mediator.Publish(
                new UpdateUserStatusNotification(notification.UserId, notification.NewStatus)
            );
        }
    }
}
