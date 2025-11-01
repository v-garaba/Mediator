using Mediators.Repository.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mediators.Tests.Helpers;

/// <summary>
/// Helper methods for Entity Framework testing
/// </summary>
public static class EntityFrameworkTestHelpers
{
    /// <summary>
    /// Wipes all data from the in-memory database
    /// </summary>
    public static async Task WipeInMemoryDatabaseAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
        
        // Delete and recreate the database
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Clears all messages from the database
    /// </summary>
    public static async Task ClearMessagesAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
        
        context.Messages.RemoveRange(context.Messages);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets the count of messages in the database
    /// </summary>
    public static async Task<int> GetMessageCountAsync(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ChatDbContext>();
        
        return await context.Messages.CountAsync();
    }

    /// <summary>
    /// Creates a service provider with in-memory database using a unique database name
    /// </summary>
    public static IServiceProvider CreateTestServiceProviderWithInMemoryDb(string? databaseName = null)
    {
        var dbName = databaseName ?? $"TestDb_{Guid.NewGuid()}";
        
        var services = new ServiceCollection();
        services.AddDbContext<ChatDbContext>(options =>
        {
            options.UseInMemoryDatabase(dbName);
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });

        return services.BuildServiceProvider();
    }
}
