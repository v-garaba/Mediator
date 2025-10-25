using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class SmsService
{
    private readonly ILogger<SmsService> _logger;

    public SmsService(ILogger<SmsService> logger)
    {
        _logger = logger;
    }

    public void SendSms(string userId, string message)
    {
        // Simulate sending SMS
        _logger.LogInformation($"[SMS] To User: {userId}, Message: {message}");
    }
}
