using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.Logging;

namespace Mediators.Notifications;

public sealed record ChangeUserStatusNotification(UserRef UserId, UserStatus NewStatus) : INotification;

public sealed class ChangeUserStatusNotificationHandler(
    ILogger<ChangeUserStatusNotificationHandler> logger,
    IStorage<UserRef, User> userRepository)
    : AbstractNotificationHandler<ChangeUserStatusNotification>
{
    private readonly ILogger<ChangeUserStatusNotificationHandler> _logger = logger.AssertNotNull();
    private readonly IStorage<UserRef, User> _userRepository = userRepository.AssertNotNull();

    public sealed override async Task HandleAsync(ChangeUserStatusNotification notification)
    {
        var user = await _userRepository.TryGetAsync(notification.UserId);
        if (user != null)
        {
            user = user with { Status = notification.NewStatus };
            await _userRepository.SetAsync(user);
            _logger.LogInformation(
                $"[USER MGMT] User {notification.UserId} status changed to {notification.NewStatus}"
            );
        }
    }
}


