using Mediators.Models;

namespace Mediators.Notifications;

public sealed record SendMessageNotification(
    UserRef SenderId,
    string Content,
    MessageType Type,
    UserRef? TargetUserId = null
) : INotification;
