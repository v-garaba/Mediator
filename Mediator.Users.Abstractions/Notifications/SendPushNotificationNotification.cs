using Mediators.Models;

namespace Mediators.Notifications;

public sealed record SendPushNotificationNotification(UserRef UserId, string Message)
    : INotification;


