using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.Logging;

namespace Mediators.NotificationHandlers;

public sealed record UpdateUserActivityNotification(UserRef UserId) : INotification;

public sealed class UpdateUserActivityNotificationHandler(
    ILogger<UpdateUserActivityNotificationHandler> logger,
    IStorage<UserRef, User> userRepository)
    : INotificationHandler<UpdateUserActivityNotification>
{
    private readonly ILogger<UpdateUserActivityNotificationHandler> _logger = logger.AssertNotNull();
    private readonly IStorage<UserRef, User> _userRepository = userRepository.AssertNotNull();

    public async Task HandleAsync(UpdateUserActivityNotification notification)
    {
        var user = await _userRepository.TryGetAsync(notification.UserId);
        if (user != null)
        {
            user = user with { LastActiveTime = DateTimeOffset.UtcNow };
            await _userRepository.SetAsync(user);
            _logger.LogInformation($"[USER MGMT] User {notification.UserId} activity updated");
        }
    }
}
