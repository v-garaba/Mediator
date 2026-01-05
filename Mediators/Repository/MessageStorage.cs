using Mediators.Models;
using System.Collections.Immutable;

namespace Mediators.Repository;

public sealed class MessageStorage : IStorage<MessageRef, ChatMessage>
{
    private readonly Dictionary<MessageRef, ChatMessage> _storage = [];

    public async Task<ChatMessage?> TryGetAsync(MessageRef id, CancellationToken ct = default)
    {
        await Task.Yield();
        _storage.TryGetValue(id.AssertNotNull(), out var message);

        return message;
    }

    public async Task SetAsync(ChatMessage model, CancellationToken ct = default)
    {
        await Task.Yield();
        _storage[model.Id] = model.AssertNotNull();
    }

    public async Task<ImmutableArray<ChatMessage>> GetAllAsync(CancellationToken ct = default)
    {
        await Task.Yield();
        return [.. _storage.Values];
    }

    public Task<int> CountAsync(CancellationToken ct = default)
    {
        return Task.FromResult(_storage.Count);
    }
}
