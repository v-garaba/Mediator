using Mediators.Models;

namespace Mediators.Notifications;

public sealed record NotifyUserOfMessageNotification(User User, ChatMessage Message) : INotification;
