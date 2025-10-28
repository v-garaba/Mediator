using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record UpdateUserActivityNotification(UserRef UserId) : INotification;
