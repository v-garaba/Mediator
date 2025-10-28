using Mediators.Messaging;
using Mediators.Messaging.Notifications;
using Mediators.Messaging.Requests;
using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class AnalyticsService
{
    private readonly ChatMediator _mediator;
    private readonly ILogger<AnalyticsService> _logger;
    private readonly Dictionary<UserRef, int> _messageCount = [];
    private readonly Dictionary<UserRef, List<string>> _statusHistory = [];

    public AnalyticsService(ChatMediator mediator, ILogger<AnalyticsService> logger)
    {
        _mediator = mediator;
        _logger = logger;

        _mediator.Subscribe<TrackMessageNotification>(TrackMessageNotification);
        _mediator.Subscribe<TrackUserStatusChangeNotification>(TrackUserStatusChange);
        _mediator.Subscribe<TrackMessageSentNotification>(TrackMessageSent);

        _mediator.RegisterHandler<GetMessageCountRequest, GetMessageCountResponse>(GetMessageCount);
    }

    private async Task TrackMessageNotification(TrackMessageNotification message)
    {
        await Task.Yield();
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

    private async Task TrackUserStatusChange(TrackUserStatusChangeNotification message)
    {
        await Task.Yield();
        if (!_statusHistory.TryGetValue(message.UserId, out List<string>? value))
        {
            value = [];
            _statusHistory[message.UserId] = value;
        }

        value.Add($"{message.Status} at {DateTimeOffset.UtcNow}");

        _logger.LogInformation(
            $"[ANALYTICS] Status change tracked for user {message.UserId}: {message.Status}"
        );
    }

    private async Task TrackMessageSent(TrackMessageSentNotification message)
    {
        await Task.Yield();
        _logger.LogInformation(
            $"[ANALYTICS] Message sent tracked for user {message.UserId}, type: {message.MessageType}"
        );
    }

    private async Task<GetMessageCountResponse> GetMessageCount(GetMessageCountRequest request)
    {
        await Task.Yield();
        int count = _messageCount.GetValueOrDefault(request.UserId, 0);
        return new GetMessageCountResponse(count);
    }
}
