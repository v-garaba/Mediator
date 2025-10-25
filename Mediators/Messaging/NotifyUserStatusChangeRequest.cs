using Mediators.Models;

namespace Mediators.Messaging;

public sealed record NotifyUserStatusChangeRequest(
    User User,
    UserStatus OldStatus,
    UserStatus NewStatus
) : IRequest;
