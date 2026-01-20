using Mediators.Models;

namespace Mediators.Notifications;

public sealed record HandlePrivateMessageNotification(ChatMessage Message) : INotification;
