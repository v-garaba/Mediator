using Mediators.Repository.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mediators.Repository.EntityFramework;

/// <summary>
/// Database context for the chat system
/// </summary>
public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options)
    {
    }

    public DbSet<ChatMessageEntity> Messages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure ChatMessage entity
        modelBuilder.Entity<ChatMessageEntity>(entity =>
        {
            entity.ToTable("Messages");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .IsRequired()
                .ValueGeneratedNever(); // We generate GUIDs in the application

            entity.Property(e => e.SenderId)
                .IsRequired();

            entity.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(4000); // SQL Server nvarchar max efficient length

            entity.Property(e => e.MessageType)
                .IsRequired();

            entity.Property(e => e.TargetUserId)
                .IsRequired(false);

            entity.Property(e => e.Timestamp)
                .IsRequired();

            // Add indexes for common queries
            entity.HasIndex(e => e.SenderId)
                .HasDatabaseName("IX_Messages_SenderId");

            entity.HasIndex(e => e.Timestamp)
                .HasDatabaseName("IX_Messages_Timestamp");

            entity.HasIndex(e => new { e.TargetUserId, e.MessageType })
                .HasDatabaseName("IX_Messages_TargetUserId_MessageType");
        });
    }
}
