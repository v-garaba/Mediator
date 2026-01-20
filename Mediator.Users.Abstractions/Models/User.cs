namespace Mediators.Models;

/// <summary>
/// Represents a user in the chat system
/// </summary>
public sealed record User(
    UserRef Id,
    string Name,
    string Email,
    DateTimeOffset LastActiveTime,
    UserStatus Status = UserStatus.Offline
);

public enum UserStatus
{
    Online,
    Away,
    Busy,
    Offline,
}

/// <summary>
/// Represents a reference to a user, identified by a unique identifier.
/// </summary>
/// <remarks>
/// This type is immutable and is primarily used to
/// uniquely identify a specific message instance.
/// </remarks>
public sealed record UserRef
{
    public Guid Id { get; init; } = Guid.NewGuid();
}


