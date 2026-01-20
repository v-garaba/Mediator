using Mediators.Models;

namespace Mediators.Notifications;

public sealed record TrackMessageSentNotification(UserRef UserId, string MessageType) : INotification;
