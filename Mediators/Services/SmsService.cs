using Mediators.Messaging;
using Mediators.Messaging.Notifications;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class SmsService
{
    private readonly ILogger<SmsService> _logger;
    private readonly ChatMediator _mediator;

    public SmsService(ChatMediator mediator, ILogger<SmsService> logger)
    {
        _logger = logger;
        _mediator = mediator;

        _mediator.Subscribe<SendSmsNotification>(SendSmsAsync);
    }

    private async Task SendSmsAsync(SendSmsNotification message)
    {
        await Task.Yield();
        // Simulate sending SMS
        _logger.LogInformation($"[SMS] To User: {message.UserId}, Message: {message.Message}");
    }
}
