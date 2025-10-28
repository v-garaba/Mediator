using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record TrackMessageNotification(UserRef UserId, MessageRef MessageId) : INotification;
