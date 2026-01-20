using Mediators.Models;
using Mediators.Repository.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mediators.Repository.EntityFramework;

/// <summary>
/// Entity Framework implementation of user notification storage
/// </summary>
public sealed class EntityFrameworkUserNotificationStorage : IStorage<UserRef, UserNotification>
{
    private readonly UserDbContext _context;

    public EntityFrameworkUserNotificationStorage(UserDbContext context)
    {
        _context = context.AssertNotNull();
    }

    public async Task<UserNotification?> TryGetAsync(UserRef id, CancellationToken ct = default)
    {
        var entity = await _context.UserNotifications
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.UserId == id.Id, ct)
            .ConfigureAwait(false);

        return entity == null ? null : MapToModel(entity);
    }

    public async Task SetAsync(UserNotification model, CancellationToken ct = default)
    {
        model.AssertNotNull();

        var entity = MapToEntity(model);

        var trackedEntity = _context.UserNotifications.Local.FirstOrDefault(n => n.UserId == entity.UserId);

        if (trackedEntity != null)
        {
            _context.Entry(trackedEntity).CurrentValues.SetValues(entity);
        }
        else
        {
            var exists = await _context.UserNotifications
                .AsNoTracking()
                .AnyAsync(n => n.UserId == entity.UserId, ct)
                .ConfigureAwait(false);

            if (exists)
            {
                _context.UserNotifications.Update(entity);
            }
            else
            {
                await _context.UserNotifications.AddAsync(entity, ct).ConfigureAwait(false);
            }
        }

        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<UserNotification>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _context.UserNotifications
            .AsNoTracking()
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return entities.Select(MapToModel).ToList();
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        return await _context.UserNotifications.CountAsync(ct).ConfigureAwait(false);
    }

    public async Task ClearAsync(CancellationToken ct = default)
    {
        _context.UserNotifications.RemoveRange(_context.UserNotifications);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    private static UserNotification MapToModel(UserNotificationEntity entity)
    {
        return new UserNotification(
            UserId: new UserRef { Id = entity.UserId },
            MessageCount: entity.MessageCount
        );
    }

    private static UserNotificationEntity MapToEntity(UserNotification model)
    {
        return new UserNotificationEntity
        {
            UserId = model.UserId.Id,
            MessageCount = model.MessageCount
        };
    }
}
