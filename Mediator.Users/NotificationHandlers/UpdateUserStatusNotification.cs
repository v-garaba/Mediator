using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mediators.Notifications;

public sealed record UpdateUserStatusNotification(UserRef UserId, UserStatus Status)
    : INotification;

public sealed class UpdateUserStatusNotificationHandler(
    IServiceProvider serviceProvider,
    ILogger<UpdateUserStatusNotificationHandler> logger,
    IStorage<UserRef, User> userRepository
) : AbstractNotificationHandler<UpdateUserStatusNotification>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider.AssertNotNull();
    private readonly ILogger<UpdateUserStatusNotificationHandler> _logger = logger.AssertNotNull();
    private readonly IStorage<UserRef, User> _userRepository = userRepository.AssertNotNull();

    public sealed override async Task HandleAsync(UpdateUserStatusNotification notification)
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

            // Lazy resolve mediator to avoid circular dependency
            var mediator = _serviceProvider.GetRequiredService<IMediator>();

            await mediator.PublishAsync(
                new NotifyUserStatusChangeNotification(user, oldStatus, notification.Status)
            );
        }
    }
}


