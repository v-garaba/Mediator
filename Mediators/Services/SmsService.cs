using Mediators.Messaging;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class SmsService
{
    private readonly ILogger<SmsService> _logger;
    private readonly MessageBus _messageBus;

    public SmsService(MessageBus messageBus, ILogger<SmsService> logger)
    {
        _logger = logger;
        _messageBus = messageBus;

        _messageBus.Subscribe<SendSmsRequest>(SendSms);
    }

    private void SendSms(SendSmsRequest message)
    {
        // Simulate sending SMS
        _logger.LogInformation($"[SMS] To User: {message.UserId}, Message: {message.Message}");
    }
}
