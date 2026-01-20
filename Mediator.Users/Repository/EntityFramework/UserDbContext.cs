using Mediators.Repository.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mediators.Repository.EntityFramework;

/// <summary>
/// Database context for the user system
/// </summary>
public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<UserNotificationEntity> UserNotifications { get; set; } = null!;
    public DbSet<UserStatusChangeEntity> UserStatusChanges { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedNever();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.LastActiveTime)
                .IsRequired();

            entity.Property(e => e.Status)
                .IsRequired();

            entity.HasIndex(e => e.Email)
                .HasDatabaseName("IX_Users_Email")
                .IsUnique();

            entity.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Users_Status");
        });

        // Configure UserNotification entity
        modelBuilder.Entity<UserNotificationEntity>(entity =>
        {
            entity.ToTable("UserNotifications");

            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId)
                .IsRequired()
                .ValueGeneratedNever();

            entity.Property(e => e.MessageCount)
                .IsRequired();
        });

        // Configure UserStatusChange entity
        modelBuilder.Entity<UserStatusChangeEntity>(entity =>
        {
            entity.ToTable("UserStatusChanges");

            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId)
                .IsRequired()
                .ValueGeneratedNever();

            entity.Property(e => e.NewStatus)
                .IsRequired();
        });
    }
}
