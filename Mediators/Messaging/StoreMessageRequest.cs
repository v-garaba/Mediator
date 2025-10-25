using Mediators.Models;

namespace Mediators.Messaging;

public sealed record StoreMessageRequest(ChatMessage Message) : IRequest;
