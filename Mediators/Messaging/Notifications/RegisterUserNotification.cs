using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.Logging;

namespace Mediators.Messaging.Notifications;

public sealed record RegisterUserNotification(User User) : INotification;

public sealed class RegisterUserNotificationHandler(
    ILogger<RegisterUserNotificationHandler> logger,
    IStorage<UserRef, User> userRepository)
    : INotificationHandler<RegisterUserNotification>
{
    private readonly ILogger<RegisterUserNotificationHandler> _logger = logger.AssertNotNull();
    private readonly IStorage<UserRef, User> _userRepository = userRepository.AssertNotNull();
    public async Task HandleAsync(RegisterUserNotification notification)
    {
        await _userRepository.SetAsync(notification.User);
        _logger.LogInformation($"[USER MGMT] User {notification.User.Name} registered");
    }
}
