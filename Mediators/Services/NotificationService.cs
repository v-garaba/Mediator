using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

/// <summary>
/// BADLY DESIGNED: This service is tightly coupled with many other components
/// </summary>
public class NotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly EmailService _emailService;
    private readonly SmsService _smsService;
    private readonly PushNotificationService _pushService;
    private readonly AnalyticsService _analyticsService;

    // BAD: Too many dependencies - violation of Single Responsibility Principle
    public NotificationService(
        ILogger<NotificationService> logger,
        EmailService emailService,
        SmsService smsService,
        PushNotificationService pushService,
        AnalyticsService analyticsService
    )
    {
        _logger = logger;
        _emailService = emailService;
        _smsService = smsService;
        _pushService = pushService;
        _analyticsService = analyticsService;
    }

    // BAD: This method knows too much about other services
    public void NotifyUserOfMessage(User user, ChatMessage message)
    {
        _logger.LogInformation($"Notifying user {user.Name} of new message");

        // BAD: Tight coupling - directly calling multiple services
        if (user.Status == UserStatus.Offline)
        {
            _emailService.SendEmail(user.Email, "New Message", message.Content);
            _smsService.SendSms(user.Id, $"New message from {message.SenderId}");
        }
        else
        {
            _pushService.SendPushNotification(user.Id, message.Content);
        }

        // BAD: Analytics logic mixed with notification logic
        _analyticsService.TrackMessageNotification(user.Id, message.Id);
    }

    // BAD: Another method with multiple responsibilities
    public void NotifyUserStatusChange(User user, UserStatus oldStatus, UserStatus newStatus)
    {
        _logger.LogInformation($"User {user.Name} status changed from {oldStatus} to {newStatus}");

        // BAD: Hard-coded business logic that should be configurable
        if (newStatus == UserStatus.Online)
        {
            _pushService.SendPushNotification(user.Id, "You are now online");
            _analyticsService.TrackUserStatusChange(user.Id, newStatus.ToString());
        }
    }
}
