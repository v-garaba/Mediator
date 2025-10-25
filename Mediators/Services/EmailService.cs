using Mediators.Messaging;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class EmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly MessageBus _messageBus;

    public EmailService(MessageBus messageBus, ILogger<EmailService> logger)
    {
        _logger = logger;
        _messageBus = messageBus;

        // Subscribing to email messages
        _messageBus.Subscribe<EmailRequest>(SendEmail);
    }

    private void SendEmail(EmailRequest email)
    {
        // Simulate sending email
        _logger.LogInformation(
            $"[EMAIL] To: {email.To}, Subject: {email.Subject}, Body: {email.Body}"
        );
    }
}
