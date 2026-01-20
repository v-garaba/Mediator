using System.Collections.Immutable;
using Mediators.Models;
using Mediators.Repository.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mediators.Repository.EntityFramework;

/// <summary>
/// Entity Framework implementation of user status change storage
/// </summary>
public sealed class EntityFrameworkUserStatusChangeStorage : IStorage<UserRef, UserStatusChange>
{
    private readonly UserDbContext _context;

    public EntityFrameworkUserStatusChangeStorage(UserDbContext context)
    {
        _context = context.AssertNotNull();
    }

    public async Task<UserStatusChange?> TryGetAsync(UserRef id, CancellationToken ct = default)
    {
        var entity = await _context.UserStatusChanges
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == id.Id, ct)
            .ConfigureAwait(false);

        return entity == null ? null : MapToModel(entity);
    }

    public async Task SetAsync(UserStatusChange model, CancellationToken ct = default)
    {
        model.AssertNotNull();

        var entity = MapToEntity(model);

        var trackedEntity = _context.UserStatusChanges.Local.FirstOrDefault(s => s.UserId == entity.UserId);

        if (trackedEntity != null)
        {
            _context.Entry(trackedEntity).CurrentValues.SetValues(entity);
        }
        else
        {
            var exists = await _context.UserStatusChanges
                .AsNoTracking()
                .AnyAsync(s => s.UserId == entity.UserId, ct)
                .ConfigureAwait(false);

            if (exists)
            {
                _context.UserStatusChanges.Update(entity);
            }
            else
            {
                await _context.UserStatusChanges.AddAsync(entity, ct).ConfigureAwait(false);
            }
        }

        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task<ImmutableArray<UserStatusChange>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _context.UserStatusChanges
            .AsNoTracking()
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return entities.Select(MapToModel).ToImmutableArray();
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        return await _context.UserStatusChanges.CountAsync(ct).ConfigureAwait(false);
    }

    public async Task ClearAsync(CancellationToken ct = default)
    {
        _context.UserStatusChanges.RemoveRange(_context.UserStatusChanges);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private static UserStatusChange MapToModel(UserStatusChangeEntity entity)
    {
        return new UserStatusChange(
            UserId: new UserRef { Id = entity.UserId },
            NewStatus: (UserStatus)entity.NewStatus
        );
    }

    private static UserStatusChangeEntity MapToEntity(UserStatusChange model)
    {
        return new UserStatusChangeEntity
        {
            UserId = model.UserId.Id,
            NewStatus = (int)model.NewStatus
        };
    }
}
