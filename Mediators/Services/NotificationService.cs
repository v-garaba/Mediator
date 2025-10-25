using Mediators.Messaging;
using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

/// <summary>
/// BADLY DESIGNED: This service is tightly coupled with many other components
/// </summary>
public class NotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly MessageBus _messageBus;

    public NotificationService(MessageBus messageBus, ILogger<NotificationService> logger)
    {
        _logger = logger;
        _messageBus = messageBus;

        _messageBus.Subscribe<NotifyUserOfMessageRequest>(NotifyUserOfMessage);
        _messageBus.Subscribe<NotifyUserStatusChangeRequest>(NotifyUserStatusChange);
    }

    private void NotifyUserOfMessage(NotifyUserOfMessageRequest request)
    {
        _logger.LogInformation($"Notifying user {request.User.Name} of new message");

        // BAD: Tight coupling - directly calling multiple services
        if (request.User.Status == UserStatus.Offline)
        {
            _messageBus.Publish(
                new EmailRequest(request.User.Email, "New Message", request.Message.Content)
            );
            _messageBus.Publish(
                new SendSmsRequest(request.User.Id, $"New message from {request.Message.SenderId}")
            );
        }
        else
        {
            _messageBus.Publish(
                new SendPushNotificationRequest(request.User.Id, request.Message.Content)
            );
        }

        _messageBus.Publish(
            new TrackMessageNotificationRequest(request.User.Id, request.Message.Id)
        );
    }

    private void NotifyUserStatusChange(NotifyUserStatusChangeRequest request)
    {
        _logger.LogInformation(
            $"User {request.User.Name} status changed from {request.OldStatus} to {request.NewStatus}"
        );

        if (request.NewStatus == UserStatus.Online)
        {
            _messageBus.Publish(
                new SendPushNotificationRequest(request.User.Id, "You are now online")
            );
            _messageBus.Publish(
                new TrackUserStatusChangeRequest(request.User.Id, request.NewStatus.ToString())
            );
        }
    }
}
