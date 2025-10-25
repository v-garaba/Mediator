using Mediators.Messaging;
using Mediators.Messaging.Notifications;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class EmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly ChatMediator _mediator;

    public EmailService(ChatMediator mediator, ILogger<EmailService> logger)
    {
        _logger = logger;
        _mediator = mediator;

        // Subscribing to email messages
        _mediator.Subscribe<EmailNotification>(SendEmailAsync);
    }

    private async Task SendEmailAsync(EmailNotification email)
    {
        await Task.Yield();
        // Simulate sending email
        _logger.LogInformation(
            $"[EMAIL] To: {email.To}, Subject: {email.Subject}, Body: {email.Body}"
        );
    }
}
