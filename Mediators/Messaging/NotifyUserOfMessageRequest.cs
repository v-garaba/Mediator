using Mediators.Models;

namespace Mediators.Messaging;

public sealed record NotifyUserOfMessageRequest(User User, ChatMessage Message) : IRequest;
