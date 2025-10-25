using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record UpdateUserStatusNotification(string UserId, UserStatus Status) : INotification;
