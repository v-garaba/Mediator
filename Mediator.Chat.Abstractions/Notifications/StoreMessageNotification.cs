using Mediators.Models;

namespace Mediators.Notifications;

public sealed record StoreMessageNotification(ChatMessage Message) : INotification;
