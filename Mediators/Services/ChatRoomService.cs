using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

/// <summary>
/// BADLY DESIGNED: This service has too many responsibilities and tight coupling
/// </summary>
public class ChatRoomService
{
    private readonly ILogger<ChatRoomService> _logger;
    private readonly NotificationService _notificationService;
    private readonly AnalyticsService _analyticsService;
    private readonly MessageStorageService _storageService;
    private readonly UserManagementService _userManagementService;

    private readonly Dictionary<string, User> _users = new();
    private readonly List<ChatMessage> _messages = new();

    // BAD: Too many dependencies
    public ChatRoomService(
        ILogger<ChatRoomService> logger,
        NotificationService notificationService,
        AnalyticsService analyticsService,
        MessageStorageService storageService,
        UserManagementService userManagementService
    )
    {
        _logger = logger;
        _notificationService = notificationService;
        _analyticsService = analyticsService;
        _storageService = storageService;
        _userManagementService = userManagementService;
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

        // BAD: Directly calling storage service
        _storageService.StoreMessage(message);

        // BAD: Directly calling analytics
        _analyticsService.TrackMessageSent(senderId, type.ToString());

        // BAD: Complex notification logic that should be elsewhere
        if (type == MessageType.Private && targetUserId != null)
        {
            if (_users.TryGetValue(targetUserId, out var targetUser))
            {
                _notificationService.NotifyUserOfMessage(targetUser, message);
            }
        }
        else if (type == MessageType.Public)
        {
            // BAD: Notifying all users in a loop - tight coupling
            foreach (var user in _users.Values)
            {
                if (user.Id != senderId)
                {
                    _notificationService.NotifyUserOfMessage(user, message);
                }
            }
        }

        // BAD: Calling user management service to update activity
        _userManagementService.UpdateUserActivity(senderId);
    }

    public void AddUser(User user)
    {
        _users[user.Id] = user;
        _logger.LogInformation($"User {user.Name} joined the chat room");

        // BAD: Tight coupling with user management
        _userManagementService.RegisterUser(user);

        // BAD: Sending system message - mixed responsibilities
        var systemMessage = new ChatMessage(
            "SYSTEM",
            $"{user.Name} joined the chat",
            MessageType.System
        );
        _messages.Add(systemMessage);
        _storageService.StoreMessage(systemMessage);
    }

    public void ChangeUserStatus(string userId, UserStatus newStatus)
    {
        if (_users.TryGetValue(userId, out var user))
        {
            var oldStatus = user.Status;
            user = user with { Status = newStatus };

            _logger.LogInformation($"User {user.Name} status changed to {newStatus}");

            // BAD: Direct coupling with notification service
            _notificationService.NotifyUserStatusChange(user, oldStatus, newStatus);

            // BAD: Direct coupling with user management
            _userManagementService.UpdateUserStatus(userId, newStatus);
        }
    }

    public IReadOnlyList<ChatMessage> GetMessages() => _messages.AsReadOnly();

    public User? GetUser(string userId) => _users.GetValueOrDefault(userId);
}
