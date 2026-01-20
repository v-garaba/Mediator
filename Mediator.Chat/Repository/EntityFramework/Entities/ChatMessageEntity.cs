namespace Mediators.Repository.EntityFramework.Entities;

/// <summary>
/// Entity Framework entity for ChatMessage
/// </summary>
public class ChatMessageEntity
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int MessageType { get; set; }
    public Guid? TargetUserId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}
