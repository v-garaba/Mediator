using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.Logging;

namespace Mediators.Messaging.Notifications;

public sealed record UpdateUserStatusNotification(UserRef UserId, UserStatus Status) : INotification;

public sealed class UpdateUserStatusNotificationHandler(
    IMediator _mediator,
    ILogger<UpdateUserStatusNotificationHandler> logger,
    IStorage<UserRef, User> userRepository)
    : INotificationHandler<UpdateUserStatusNotification>
{
    private readonly IMediator _mediator = _mediator.AssertNotNull();
    private readonly ILogger<UpdateUserStatusNotificationHandler> _logger = logger.AssertNotNull();
    private readonly IStorage<UserRef, User> _userRepository = userRepository.AssertNotNull();

    public async Task HandleAsync(UpdateUserStatusNotification notification)
    {
        var user = await _userRepository.TryGetAsync(notification.UserId);
        if (user != null)
        {
            var oldStatus = user.Status;
            user = user with { Status = notification.Status };
            await _userRepository.SetAsync(user);
            _logger.LogInformation(
                $"[USER MGMT] User {notification.UserId} status updated to {notification.Status}"
            );

            await _mediator.PublishAsync(
                new NotifyUserStatusChangeNotification(user, oldStatus, notification.Status)
            );
            await _mediator.PublishAsync(
                new UpdateUserStatusNotification(notification.UserId, notification.Status)
            );
        }
    }
}
