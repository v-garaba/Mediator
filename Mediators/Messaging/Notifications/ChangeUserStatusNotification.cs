using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record ChangeUserStatusNotification(string UserId, UserStatus NewStatus)
    : INotification;
