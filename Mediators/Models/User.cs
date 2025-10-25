namespace Mediators.Models;

/// <summary>
/// Represents a user in the chat system
/// </summary>
public sealed record User(
    string Id,
    string Name,
    string Email,
    DateTime LastActiveTime,
    UserStatus Status = UserStatus.Offline
);

public enum UserStatus
{
    Online,
    Away,
    Busy,
    Offline,
}
