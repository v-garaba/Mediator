namespace Mediators.Messaging.Notifications;

/// <summary>
/// Marker interface for messages
/// </summary>
public sealed record EmailNotification(string To, string Subject, string Body) : INotification;
