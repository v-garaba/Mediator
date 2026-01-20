using System.Net.Http.Json;
using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Clients;

public sealed class UserApiClient(HttpClient httpClient, ILogger<UserApiClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<UserApiClient> _logger = logger;

    public async Task<User> CreateUserAsync(User user)
    {
        var request = new CreateUserRequest(user.Name, user.Email, user.Id.Id);
        var response = await _httpClient.PostAsJsonAsync("/users", request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<User>().ConfigureAwait(false)
                     ?? throw new InvalidOperationException("Failed to deserialize created user");
        _logger.LogInformation("[USER API] Created user {UserId}", created.Id.Id);
        return created;
    }

    public async Task<User?> GetUserAsync(UserRef userId)
    {
        var response = await _httpClient.GetAsync($"/users/{userId.Id}").ConfigureAwait(false);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<User>().ConfigureAwait(false);
    }

    public async Task<User> UpdateStatusAsync(UserRef userId, UserStatus status)
    {
        var request = new UpdateStatusRequest(status);
        var response = await _httpClient.PutAsJsonAsync($"/users/{userId.Id}/status", request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var updated = await response.Content.ReadFromJsonAsync<User>().ConfigureAwait(false)
                       ?? throw new InvalidOperationException("Failed to deserialize updated user");
        _logger.LogInformation("[USER API] Updated status for user {UserId} to {Status}", userId.Id, status);
        return updated;
    }

    public async Task<UserNotification?> GetNotificationAsync(UserRef userId)
    {
        var response = await _httpClient.GetAsync($"/users/{userId.Id}/notifications").ConfigureAwait(false);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserNotification>().ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<User>> GetUsersAsync()
    {
        var response = await _httpClient.GetAsync("/users").ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<IReadOnlyList<User>>().ConfigureAwait(false)
                    ?? Array.Empty<User>();
        return users;
    }

    public async Task<UserNotification> NotifyUserAsync(UserRef userId, UserRef senderId, string message)
    {
        var request = new NotifyUserRequest(senderId.Id, message);
        var response = await _httpClient.PostAsJsonAsync($"/users/{userId.Id}/notify", request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var updated = await response.Content.ReadFromJsonAsync<UserNotification>().ConfigureAwait(false)
                      ?? throw new InvalidOperationException("Failed to deserialize notification update");
        _logger.LogInformation("[USER API] Notified user {UserId}; total notifications {Count}", userId.Id, updated.MessageCount);
        return updated;
    }

    public async Task ResetAsync()
    {
        var response = await _httpClient.PostAsync("/reset", null).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        _logger.LogInformation("[USER API] Reset complete");
    }

    private sealed record CreateUserRequest(string Name, string Email, Guid? Id);
    private sealed record UpdateStatusRequest(UserStatus Status);
    private sealed record NotifyUserRequest(Guid SenderId, string Message);
}
