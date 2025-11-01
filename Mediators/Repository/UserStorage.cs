using Mediators.Data;
using Mediators.Models;

namespace Mediators.Repository;

public sealed class UserStorage : IStorage<UserRef, User>
{
    private readonly Dictionary<UserRef, User> _storage = [];

    public UserStorage()
    {
        // Prepopulate system user
        var systemRef = ChatSystem.Reference;
        _storage[systemRef] = new User(
            systemRef,
            "System",
            "system@mediator.com",
            DateTimeOffset.UtcNow,
            UserStatus.Online
        );
    }

    public async Task<User?> TryGetAsync(UserRef id, CancellationToken ct = default)
    {
        await Task.Yield();
        _storage.TryGetValue(id.AssertNotNull(), out var user);

        return user;
    }

    public async Task SetAsync(User model, CancellationToken ct = default)
    {
        await Task.Yield();
        _storage[model.Id] = model.AssertNotNull();
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default)
    {
        await Task.Yield();
        return [.. _storage.Values];
    }

    public Task<int> CountAsync(CancellationToken ct = default)
    {
        return Task.FromResult(_storage.Count);
    }
}
