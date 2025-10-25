using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record NotifyUserStatusChangeNotification(
    User User,
    UserStatus OldStatus,
    UserStatus NewStatus
) : INotification;
