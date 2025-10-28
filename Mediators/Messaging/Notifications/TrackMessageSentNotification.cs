using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record TrackMessageSentNotification(UserRef UserId, string MessageType)
    : INotification;
