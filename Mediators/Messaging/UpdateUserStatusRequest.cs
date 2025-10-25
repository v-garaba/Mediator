using Mediators.Models;

namespace Mediators.Messaging;

public sealed record UpdateUserStatusRequest(string UserId, UserStatus Status) : IRequest;
