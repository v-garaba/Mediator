namespace Mediators.Notifications;

/// <summary>
/// Notification to send an email
/// </summary>
public sealed record EmailNotification(string To, string Subject, string Body) : INotification;


