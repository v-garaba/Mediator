using Mediators.Models;

namespace Mediators.Messaging.Notifications;

public sealed record SendSmsNotification(UserRef UserId, string Message) : INotification;
