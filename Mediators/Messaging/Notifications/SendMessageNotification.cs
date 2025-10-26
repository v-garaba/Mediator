using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record SendMessageNotification(
    string SenderId,
    string Content,
    MessageType Type,
    string? TargetUserId = null
) : INotification;
