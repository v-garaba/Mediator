namespace Mediators.Messaging;

/// <summary>
/// Marker interface for messages
/// </summary>
public sealed record EmailRequest(string To, string Subject, string Body) : IRequest;
