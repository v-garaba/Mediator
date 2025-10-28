using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record UpdateUserStatusNotification(UserRef UserId, UserStatus Status) : INotification;
