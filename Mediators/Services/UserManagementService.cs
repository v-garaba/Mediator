using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Services;

public class UserManagementService
{
    private readonly ILogger<UserManagementService> _logger;
    private readonly Dictionary<string, User> _registeredUsers = new();

    public UserManagementService(ILogger<UserManagementService> logger)
    {
        _logger = logger;
    }

    public void RegisterUser(User user)
    {
        _registeredUsers[user.Id] = user;
        _logger.LogInformation($"[USER MGMT] User {user.Name} registered");
    }

    public void UpdateUserActivity(string userId)
    {
        if (_registeredUsers.TryGetValue(userId, out var user))
        {
            user = user with { LastActiveTime = DateTime.UtcNow };
            _logger.LogInformation($"[USER MGMT] User {userId} activity updated");
        }
    }

    public void UpdateUserStatus(string userId, UserStatus status)
    {
        if (_registeredUsers.TryGetValue(userId, out var user))
        {
            user = user with { Status = status };
            _logger.LogInformation($"[USER MGMT] User {userId} status updated to {status}");
        }
    }

    public User? GetUser(string userId) => _registeredUsers.GetValueOrDefault(userId);
}
