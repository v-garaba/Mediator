namespace Mediators.Models;

public sealed record UserNotification(UserRef UserId, int MessageCount);