using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record NotifyUserOfMessageNotification(User User, ChatMessage Message)
    : INotification;
