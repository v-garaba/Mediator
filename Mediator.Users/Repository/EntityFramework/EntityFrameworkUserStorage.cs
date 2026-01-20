using Mediators.Data;
using Mediators.Models;
using Mediators.Repository.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mediators.Repository.EntityFramework;

/// <summary>
/// Entity Framework implementation of user storage
/// </summary>
public sealed class EntityFrameworkUserStorage : IStorage<UserRef, User>
{
    private readonly UserDbContext _context;

    public EntityFrameworkUserStorage(UserDbContext context)
    {
        _context = context.AssertNotNull();
        EnsureSystemUserExists();
    }

    private void EnsureSystemUserExists()
    {
        var systemRef = ChatSystem.Reference;
        var exists = _context.Users.Any(u => u.Id == systemRef.Id);
        if (!exists)
        {
            var systemUser = new UserEntity
            {
                Id = systemRef.Id,
                Name = "System",
                Email = "system@mediator.com",
                LastActiveTime = DateTimeOffset.UtcNow,
                Status = (int)UserStatus.Online
            };
            _context.Users.Add(systemUser);
            _context.SaveChanges();
        }
    }

    public async Task<User?> TryGetAsync(UserRef id, CancellationToken ct = default)
    {
        var entity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id.Id, ct)
            .ConfigureAwait(false);

        return entity == null ? null : MapToModel(entity);
    }

    public async Task SetAsync(User model, CancellationToken ct = default)
    {
        model.AssertNotNull();

        var entity = MapToEntity(model);

        var trackedEntity = _context.Users.Local.FirstOrDefault(u => u.Id == entity.Id);

        if (trackedEntity != null)
        {
            _context.Entry(trackedEntity).CurrentValues.SetValues(entity);
        }
        else
        {
            var exists = await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Id == entity.Id, ct)
                .ConfigureAwait(false);

            if (exists)
            {
                _context.Users.Update(entity);
            }
            else
            {
                await _context.Users.AddAsync(entity, ct).ConfigureAwait(false);
            }
        }

        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _context.Users
            .AsNoTracking()
            .OrderBy(u => u.Name)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        return entities.Select(MapToModel).ToList();
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
    {
        return await _context.Users.CountAsync(ct).ConfigureAwait(false);
    }

    public async Task ClearAsync(CancellationToken ct = default)
    {
        _context.Users.RemoveRange(_context.Users);
        await _context.SaveChangesAsync(ct).ConfigureAwait(false);
        EnsureSystemUserExists();
    }

    private static User MapToModel(UserEntity entity)
    {
        return new User(
            Id: new UserRef { Id = entity.Id },
            Name: entity.Name,
            Email: entity.Email,
            LastActiveTime: entity.LastActiveTime,
            Status: (UserStatus)entity.Status
        );
    }

    private static UserEntity MapToEntity(User model)
    {
        return new UserEntity
        {
            Id = model.Id.Id,
            Name = model.Name,
            Email = model.Email,
            LastActiveTime = model.LastActiveTime,
            Status = (int)model.Status
        };
    }
}
