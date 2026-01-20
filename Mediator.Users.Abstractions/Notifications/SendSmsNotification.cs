using Mediators.Models;

namespace Mediators.Notifications;

public sealed record SendSmsNotification(UserRef UserId, string Message) : INotification;


