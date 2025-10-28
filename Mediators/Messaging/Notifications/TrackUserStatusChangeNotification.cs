using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record TrackUserStatusChangeNotification(UserRef UserId, string Status)
    : INotification;
