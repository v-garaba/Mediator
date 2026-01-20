using Mediators.Models;

namespace Mediators.Clients;

public interface IChatRoom
{
    Task AddUserAsync(User user);
    Task ChangeUserStatusAsync(UserRef userId, UserStatus newStatus);
    Task SendMessageAsync(UserRef senderId, string content, MessageType type, UserRef? targetUserId = null);
}
