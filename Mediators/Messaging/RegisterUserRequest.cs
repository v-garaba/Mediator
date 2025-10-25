using Mediators.Models;

namespace Mediators.Messaging;

public sealed record RegisterUserRequest(User User) : IRequest;
