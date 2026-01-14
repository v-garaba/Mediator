using Mediators.Models;
using System.Collections.Immutable;

namespace Mediators.Repository;

public sealed class UserNotificationStorage : IStorage<UserRef, UserNotification>
{
    private readonly Dictionary<UserRef, UserNotification> _storage = new();

    public async Task<UserNotification?> TryGetAsync(UserRef id, CancellationToken ct = default)
    {
        await Task.Yield();
        _storage.TryGetValue(id.AssertNotNull(), out var notification);

        return notification;
    }

    public async Task SetAsync(UserNotification model, CancellationToken ct = default)
    {
        await Task.Yield();
        _storage[model.UserId] = model.AssertNotNull();
    }

    public async Task<ImmutableArray<UserNotification>> GetAllAsync(CancellationToken ct = default)
    {
        await Task.Yield();
        return [.. _storage.Values];
    }

    public Task<int> CountAsync(CancellationToken ct = default)
    {
        return Task.FromResult(_storage.Count);
    }
}
