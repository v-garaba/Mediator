namespace Mediators.Models;

/// <summary>
/// Represents a chat message
/// </summary>
public sealed record ChatMessage(
    string SenderId,
    string Content,
    MessageType Type,
    string? TargetUserId = null
)
{
    private Guid InternalId { get; init; } = Guid.NewGuid();
    public string Id => InternalId.ToString();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public enum MessageType
{
    Public,
    Private,
    System,
}
