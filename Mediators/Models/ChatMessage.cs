namespace Mediators.Models;

/// <summary>
/// Represents a chat message
/// </summary>
public class ChatMessage
{
    public string Id { get; init; }
    public string SenderId { get; init; }
    public string Content { get; init; }
    public DateTime Timestamp { get; init; }
    public MessageType Type { get; init; }
    public string? TargetUserId { get; init; } // For private messages

    public ChatMessage(string senderId, string content, MessageType type, string? targetUserId = null)
    {
        Id = Guid.NewGuid().ToString();
        SenderId = senderId;
        Content = content;
        Type = type;
        TargetUserId = targetUserId;
        Timestamp = DateTime.UtcNow;
    }
}

public enum MessageType
{
    Public,
    Private,
    System
}
