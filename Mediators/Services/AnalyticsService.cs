using Mediators.Messaging;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class AnalyticsService
{
    private readonly MessageBus _messageBus;
    private readonly ILogger<AnalyticsService> _logger;
    private readonly Dictionary<string, int> _messageCount = [];
    private readonly Dictionary<string, List<string>> _statusHistory = [];

    public AnalyticsService(MessageBus messageBus, ILogger<AnalyticsService> logger)
    {
        _messageBus = messageBus;
        _logger = logger;

        _messageBus.Subscribe<TrackMessageNotificationRequest>(TrackMessageNotification);
        _messageBus.Subscribe<TrackUserStatusChangeRequest>(TrackUserStatusChange);
        _messageBus.Subscribe<TrackMessageSentRequest>(TrackMessageSent);
    }

    private void TrackMessageNotification(TrackMessageNotificationRequest message)
    {
        if (!_messageCount.TryGetValue(message.UserId, out int value))
        {
            value = 0;
            _messageCount[message.UserId] = value;
        }
        _messageCount[message.UserId] = ++value;

        _logger.LogInformation(
            $"[ANALYTICS] Message notification tracked for user {message.UserId}. Total: {value}"
        );
    }

    private void TrackUserStatusChange(TrackUserStatusChangeRequest message)
    {
        if (!_statusHistory.TryGetValue(message.UserId, out List<string>? value))
        {
            value = [];
            _statusHistory[message.UserId] = value;
        }

        value.Add($"{message.Status} at {DateTime.UtcNow}");

        _logger.LogInformation(
            $"[ANALYTICS] Status change tracked for user {message.UserId}: {message.Status}"
        );
    }

    private void TrackMessageSent(TrackMessageSentRequest message)
    {
        _logger.LogInformation(
            $"[ANALYTICS] Message sent tracked for user {message.UserId}, type: {message.MessageType}"
        );
    }

    public int GetMessageCount(string userId)
    {
        return _messageCount.GetValueOrDefault(userId, 0);
    }
}
