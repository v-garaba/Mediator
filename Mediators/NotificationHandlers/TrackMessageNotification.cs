using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.Logging;

namespace Mediators.NotificationHandlers;

public sealed record TrackMessageNotification(UserRef UserId, MessageRef MessageId) : INotification;

public sealed class TrackMessageNotificationHandler(
    ILogger<TrackMessageNotificationHandler> logger,
    IStorage<UserRef, UserNotification> userNotificationStorage
) : INotificationHandler<TrackMessageNotification>
{
    private readonly ILogger<TrackMessageNotificationHandler> _logger = logger.AssertNotNull();
    private readonly IStorage<UserRef, UserNotification> _userNotificationStorage =
        userNotificationStorage.AssertNotNull();

    public async Task HandleAsync(TrackMessageNotification notification)
    {
        notification.AssertNotNull();
        var userNotification =
            await _userNotificationStorage.TryGetAsync(notification.UserId)
            ?? new UserNotification(notification.UserId, 0);

        await _userNotificationStorage.SetAsync(
            userNotification with
            {
                MessageCount = userNotification.MessageCount + 1,
            }
        );

        _logger.LogInformation(
            $"[ANALYTICS] Message notification tracked for user {notification.UserId}. Total: {userNotification.MessageCount}"
        );
    }
}
