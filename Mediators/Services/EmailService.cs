using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class EmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public void SendEmail(string to, string subject, string body)
    {
        // Simulate sending email
        _logger.LogInformation($"[EMAIL] To: {to}, Subject: {subject}, Body: {body}");
    }
}
