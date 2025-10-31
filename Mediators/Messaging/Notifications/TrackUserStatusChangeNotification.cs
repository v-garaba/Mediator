﻿using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.Logging;

namespace Mediators.Messaging.Notifications;

public sealed record TrackUserStatusChangeNotification(UserRef UserId, UserStatus Status)
    : INotification;

public sealed class TrackUserStatusChangeNotificationHandler(
    ILogger<TrackUserStatusChangeNotificationHandler> logger,
    IStorage<UserRef, UserStatusChange> userStatusChangeStorage)
    : INotificationHandler<TrackUserStatusChangeNotification>
{
    private readonly ILogger<TrackUserStatusChangeNotificationHandler> _logger = logger.AssertNotNull();
    private readonly IStorage<UserRef, UserStatusChange> _userStatusChangeStorage = userStatusChangeStorage.AssertNotNull();

    public async Task HandleAsync(TrackUserStatusChangeNotification notification)
    {
        await Task.Yield();
        notification.AssertNotNull();
        var statusChange = await _userStatusChangeStorage.TryGetAsync(notification.UserId)
            ?? new UserStatusChange(notification.UserId, notification.Status);
        await _userStatusChangeStorage.SetAsync(statusChange);

        _logger.LogInformation(
            $"[ANALYTICS] Status change tracked for user {notification.UserId}: {notification.Status}"
        );
    }
}
