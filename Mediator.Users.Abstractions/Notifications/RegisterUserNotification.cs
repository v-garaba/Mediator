using Mediators.Models;

namespace Mediators.Notifications;

public sealed record RegisterUserNotification(User User) : INotification;


