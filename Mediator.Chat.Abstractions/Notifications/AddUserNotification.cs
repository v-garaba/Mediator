using Mediators.Models;

namespace Mediators.Notifications;

public sealed record AddUserNotification(User User) : INotification;
