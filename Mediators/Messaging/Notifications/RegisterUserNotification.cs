using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record RegisterUserNotification(User User) : INotification;
