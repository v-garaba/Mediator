using Mediators.Models;

namespace Mediators.Notifications;

public sealed record HandlePublicMessageNotification(ChatMessage Message) : INotification;
