namespace Mediators.Models;

public sealed record UserStatusChange(UserRef UserId, UserStatus NewStatus);

