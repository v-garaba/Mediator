using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record ChangeUserStatusNotification(UserRef UserId, UserStatus NewStatus)
    : INotification;
