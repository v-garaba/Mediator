using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record SendPushNotificationNotification(UserRef UserId, string Message)
    : INotification;
