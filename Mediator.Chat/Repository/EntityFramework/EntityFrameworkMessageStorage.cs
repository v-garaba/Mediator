using System.Collections.Immutable;
using Mediators.Models;
using Mediators.Repository.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mediators.Repository.EntityFramework;

/// <summary>
/// Entity Framework implementation of message storage
/// </summary>
public sealed class EntityFrameworkMessageStorage : IStorage<MessageRef, ChatMessage>
{
    private readonly ChatDbContext _context;

    public EntityFrameworkMessageStorage(ChatDbContext context)
    {
        _context = context.AssertNotNull();
    }

    public async Task<ChatMessage?> TryGetAsync(MessageRef id, CancellationToken ct = default)
    {
        var entity = await _context.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id.Id, ct)
            .ConfigureAwait(false);

        return entity == null ? null : MapToModel(entity);
    }

    public async Task SetAsync(ChatMessage model, CancellationToken ct = default)
    {
        model.AssertNotNull();

        var entity = MapToEntity(model);

        // Try to find the entity in the change tracker first
        var trackedEntity = _context.Messages.Local.FirstOrDefault(m => m.Id == entity.Id);

        if (trackedEntity != null)
        {
            // Update the tracked entity
            _context.Entry(trackedEntity).CurrentValues.SetValues(entity);
        }
        else
        {
            // Check if message already exists in database (use AsNoTracking to avoid tracking issues)
            var exists = await _context.Messages
                .AsNoTracking()
                .AnyAsync(m => m.Id == entity.Id, ct)
                .ConfigureAwait(false);

            if (exists)
            {
                _context.Messages.Update(entity);
            }
            else
            {
                await _context.Messages.AddAsync(entity, ct).ConfigureAwait(false);
            }
        }

        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task<ImmutableArray<ChatMessage>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _context.Messages
            .AsNoTracking()
            .OrderBy(m => m.Timestamp)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return entities.Select(MapToModel).ToImmutableArray();
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        return await _context.Messages.CountAsync(ct).ConfigureAwait(false);
    }

    public async Task ClearAsync(CancellationToken ct = default)
    {
        _context.Messages.RemoveRange(_context.Messages);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Maps entity to domain model
    /// </summary>
    private static ChatMessage MapToModel(ChatMessageEntity entity)
    {
        return new ChatMessage(
            SenderId: new UserRef { Id = entity.SenderId },
            Content: entity.Content,
            Type: (MessageType)entity.MessageType,
            TargetUserId: entity.TargetUserId.HasValue
                ? new UserRef { Id = entity.TargetUserId.Value }
                : null
        )
        {
            Id = new MessageRef { Id = entity.Id },
            Timestamp = entity.Timestamp
        };
    }

    /// <summary>
    /// Maps domain model to entity
    /// </summary>
    private static ChatMessageEntity MapToEntity(ChatMessage model)
    {
        return new ChatMessageEntity
        {
            Id = model.Id.Id,
            SenderId = model.SenderId.Id,
            Content = model.Content,
            MessageType = (int)model.Type,
            TargetUserId = model.TargetUserId?.Id,
            Timestamp = model.Timestamp
        };
    }
}
