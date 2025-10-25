using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class AnalyticsService
{
    private readonly ILogger<AnalyticsService> _logger;
    private readonly Dictionary<string, int> _messageCount = new();
    private readonly Dictionary<string, List<string>> _statusHistory = new();

    public AnalyticsService(ILogger<AnalyticsService> logger)
    {
        _logger = logger;
    }

    public void TrackMessageNotification(string userId, string messageId)
    {
        if (!_messageCount.ContainsKey(userId))
        {
            _messageCount[userId] = 0;
        }
        _messageCount[userId]++;

        _logger.LogInformation(
            $"[ANALYTICS] Message notification tracked for user {userId}. Total: {_messageCount[userId]}"
        );
    }

    public void TrackUserStatusChange(string userId, string status)
    {
        if (!_statusHistory.ContainsKey(userId))
        {
            _statusHistory[userId] = new List<string>();
        }
        _statusHistory[userId].Add($"{status} at {DateTime.UtcNow}");

        _logger.LogInformation($"[ANALYTICS] Status change tracked for user {userId}: {status}");
    }

    public void TrackMessageSent(string userId, string messageType)
    {
        _logger.LogInformation(
            $"[ANALYTICS] Message sent tracked for user {userId}, type: {messageType}"
        );
    }

    public int GetMessageCount(string userId)
    {
        return _messageCount.GetValueOrDefault(userId, 0);
    }
}
