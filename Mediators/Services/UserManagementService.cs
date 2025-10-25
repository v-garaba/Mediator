using Mediators.Messaging;
using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class UserManagementService
{
    private readonly MessageBus _messageBus;
    private readonly ILogger<UserManagementService> _logger;
    private readonly Dictionary<string, User> _registeredUsers = [];

    public UserManagementService(MessageBus messageBus, ILogger<UserManagementService> logger)
    {
        _logger = logger;
        _messageBus = messageBus;

        _messageBus.Subscribe<RegisterUserRequest>(RegisterUser);
        _messageBus.Subscribe<UpdateUserActivityRequest>(UpdateUserActivity);
        _messageBus.Subscribe<UpdateUserStatusRequest>(UpdateUserStatus);
    }

    private void RegisterUser(RegisterUserRequest request)
    {
        _registeredUsers[request.User.Id] = request.User;
        _logger.LogInformation($"[USER MGMT] User {request.User.Name} registered");
    }

    private void UpdateUserActivity(UpdateUserActivityRequest request)
    {
        if (_registeredUsers.TryGetValue(request.UserId, out var user))
        {
            user = user with { LastActiveTime = DateTime.UtcNow };
            _logger.LogInformation($"[USER MGMT] User {request.UserId} activity updated");
        }
    }

    private void UpdateUserStatus(UpdateUserStatusRequest request)
    {
        if (_registeredUsers.TryGetValue(request.UserId, out var user))
        {
            user = user with { Status = request.Status };
            _registeredUsers[request.UserId] = user;
            _logger.LogInformation(
                $"[USER MGMT] User {request.UserId} status updated to {request.Status}"
            );
        }
    }

    public User? GetUser(string userId) => _registeredUsers.GetValueOrDefault(userId);
}
