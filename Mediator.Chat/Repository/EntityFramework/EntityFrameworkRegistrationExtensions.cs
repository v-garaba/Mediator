using Mediators.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mediators.Repository.EntityFramework;

public static class EntityFrameworkRegistrationExtensions
{
    /// <summary>
    /// Registers Entity Framework storage services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="useInMemory">Whether to use in-memory database</param>
    /// <param name="connectionString">Database connection string (ignored if useInMemory is true)</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddEntityFrameworkStorage(
        this IServiceCollection services,
        bool useInMemory,
        string connectionString)
    {
        if (useInMemory)
        {
            return services.AddInMemoryEntityFrameworkStorage();
        }
        else
        {
            return services.AddSqlServerEntityFrameworkStorage(connectionString);
        }
    }

    /// <summary>
    /// Registers Entity Framework storage services with SQL Server
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="connectionString">Database connection string</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddSqlServerEntityFrameworkStorage(
        this IServiceCollection services,
        string connectionString)
    {
        // Register DbContext with SQL Server
        services.AddDbContext<ChatDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);

                sqlOptions.CommandTimeout(30);
            });

            // Enable sensitive data logging in development
            #if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            #endif
        });

        // Register the Entity Framework storage implementation
        services.AddScoped<IStorage<MessageRef, ChatMessage>, EntityFrameworkMessageStorage>();

        return services;
    }

    /// <summary>
    /// Registers Entity Framework storage services with in-memory database (for testing)
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="databaseName">In-memory database name</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddInMemoryEntityFrameworkStorage(
        this IServiceCollection services,
        string databaseName = "ChatDb")
    {
        // Register DbContext with in-memory database
        services.AddDbContext<ChatDbContext>(options =>
        {
            options.UseInMemoryDatabase(databaseName);

            #if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            #endif
        });

        // Register the Entity Framework storage implementation
        services.AddScoped<IStorage<MessageRef, ChatMessage>, EntityFrameworkMessageStorage>();

        return services;
    }

    /// <summary>
    /// Ensures the database is created and applies any pending migrations
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <returns>Task</returns>
    public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

        // This will create the database if it doesn't exist
        // For production, you should use migrations instead
        await context.Database.EnsureCreatedAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Applies any pending database migrations
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <returns>Task</returns>
    public static async Task MigrateDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();

        // Apply any pending migrations
        await context.Database.MigrateAsync().ConfigureAwait(false);
    }
}
