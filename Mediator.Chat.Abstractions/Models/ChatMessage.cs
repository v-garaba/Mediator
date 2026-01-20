namespace Mediators.Models;

/// <summary>
/// Represents a chat message
/// </summary>
public sealed record ChatMessage(
    UserRef SenderId,
    string Content,
    MessageType Type,
    UserRef? TargetUserId = null
)
{
    public MessageRef Id { get; init; } = new MessageRef();
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}

public enum MessageType
{
    Public,
    Private,
    System,
}

/// <summary>
/// Represents a reference to a message, identified by a unique identifier.
/// </summary>
/// <remarks>
/// This type is immutable and is primarily used to
/// uniquely identify a specific message instance.
/// </remarks>
public sealed record MessageRef
{
    public Guid Id { get; init; } = Guid.NewGuid();
}
