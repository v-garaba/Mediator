using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record SendMessageNotification(
    UserRef SenderId,
    string Content,
    MessageType Type,
    UserRef? TargetUserId = null
) : INotification;
