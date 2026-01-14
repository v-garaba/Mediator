using Mediators.Models;
using System.Collections.Immutable;

namespace Mediators.Repository;

public sealed class UserStatusChangeStorage : IStorage<UserRef, UserStatusChange>
{
    private readonly Dictionary<UserRef, UserStatusChange> _storage = new();

    public async Task<UserStatusChange?> TryGetAsync(UserRef id, CancellationToken ct = default)
    {
        await Task.Yield();
        _storage.TryGetValue(id.AssertNotNull(), out var statusChange);

        return statusChange;
    }

    public async Task SetAsync(UserStatusChange model, CancellationToken ct = default)
    {
        await Task.Yield();
        _storage[model.UserId] = model.AssertNotNull();
    }

    public async Task<ImmutableArray<UserStatusChange>> GetAllAsync(CancellationToken ct = default)
    {
        await Task.Yield();
        return [.. _storage.Values];
    }

    public Task<int> CountAsync(CancellationToken ct = default)
    {
        return Task.FromResult(_storage.Count);
    }
}
