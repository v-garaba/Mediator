using Mediators.Messaging;
using Mediators.Messaging.Notifications;
using Mediators.Messaging.Requests;
using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class UserManagementService
{
    private readonly ChatMediator _mediator;
    private readonly ILogger<UserManagementService> _logger;
    private readonly Dictionary<string, User> _registeredUsers = [];

    public UserManagementService(ChatMediator mediator, ILogger<UserManagementService> logger)
    {
        _logger = logger;
        _mediator = mediator;

        _mediator.Subscribe<RegisterUserNotification>(RegisterUserAsync);
        _mediator.Subscribe<UpdateUserActivityNotification>(UpdateUserActivityAsync);
        _mediator.Subscribe<UpdateUserStatusNotification>(UpdateUserStatusAsync);

        _mediator.RegisterHandler<GetUserRequest, GetUserResponse>(GetUser);
    }

    private async Task RegisterUserAsync(RegisterUserNotification Notification)
    {
        await Task.Yield();
        _registeredUsers[Notification.User.Id] = Notification.User;
        _logger.LogInformation($"[USER MGMT] User {Notification.User.Name} registered");
    }

    private async Task UpdateUserActivityAsync(UpdateUserActivityNotification Notification)
    {
        await Task.Yield();
        if (_registeredUsers.TryGetValue(Notification.UserId, out var user))
        {
            user = user with { LastActiveTime = DateTime.UtcNow };
            _registeredUsers[Notification.UserId] = user;
            _logger.LogInformation($"[USER MGMT] User {Notification.UserId} activity updated");
        }
    }

    private async Task UpdateUserStatusAsync(UpdateUserStatusNotification Notification)
    {
        await Task.Yield();
        if (_registeredUsers.TryGetValue(Notification.UserId, out var user))
        {
            user = user with { Status = Notification.Status };
            _registeredUsers[Notification.UserId] = user;
            _logger.LogInformation(
                $"[USER MGMT] User {Notification.UserId} status updated to {Notification.Status}"
            );
        }
    }

    private async Task<GetUserResponse> GetUser(GetUserRequest request)
    {
        await Task.Yield();
        return new GetUserResponse(_registeredUsers.GetValueOrDefault(request.UserId));
    }
}
