using Mediators.Models;

namespace Mediators.Notifications;

public sealed record TrackMessageNotification(UserRef UserId, MessageRef MessageId) : INotification;
