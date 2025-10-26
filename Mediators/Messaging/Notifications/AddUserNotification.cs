using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record AddUserNotification(User User) : INotification;
