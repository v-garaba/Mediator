using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Clients;

/// <summary>
/// HTTP-based chat room that talks to User and Chat APIs.
/// </summary>
public sealed class ChatRoom(UserApiClient userApiClient, ChatApiClient chatApiClient, ILogger<ChatRoom> logger) : IChatRoom
{
    private readonly ILogger<ChatRoom> _logger = logger;
    private readonly UserApiClient _userApiClient = userApiClient;
    private readonly ChatApiClient _chatApiClient = chatApiClient;

    public async Task AddUserAsync(User user)
    {
        _logger.LogInformation("[CHAT ROOM HTTP] User {User} is joining the chat room", user.Name);
        await _userApiClient.CreateUserAsync(user);
    }

    public async Task ChangeUserStatusAsync(UserRef userId, UserStatus newStatus)
    {
        _logger.LogInformation("[CHAT ROOM HTTP] User {UserId} is changing status to {Status}", userId.Id, newStatus);
        await _userApiClient.UpdateStatusAsync(userId, newStatus);
    }

    public async Task SendMessageAsync(
        UserRef senderId,
        string content,
        MessageType type,
        UserRef? targetUserId = null
    )
    {
        _logger.LogInformation(
            "[CHAT ROOM HTTP] User {UserId} is sending a {Kind} message",
            senderId.Id,
            type == MessageType.Private ? "private" : "public"
        );

        await _chatApiClient.SendMessageAsync(senderId, content, type, targetUserId);

        var users = await _userApiClient.GetUsersAsync();
        var recipients = type switch
        {
            MessageType.Public => users.Where(u => u.Id != senderId).ToList(),
            MessageType.Private when targetUserId is not null => users.Where(u => u.Id == targetUserId).ToList(),
            _ => [],
        };

        foreach (var recipient in recipients)
        {
            await _userApiClient.NotifyUserAsync(recipient.Id, senderId, content);
        }
    }
}
