using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record StoreMessageNotification(ChatMessage Message) : INotification;
