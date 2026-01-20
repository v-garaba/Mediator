using Mediators.Models;

namespace Mediators.Notifications;

public sealed record UpdateUserActivityNotification(UserRef UserId) : INotification;


