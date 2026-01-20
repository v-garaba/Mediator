using Mediators.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mediators.Repository.EntityFramework;

public static class UserEntityFrameworkRegistrationExtensions
{
    /// <summary>
    /// Registers Entity Framework storage services for users
    /// </summary>
    public static IServiceCollection AddUserEntityFrameworkStorage(
        this IServiceCollection services,
        bool useInMemory,
        string connectionString)
    {
        if (useInMemory)
        {
            return services.AddInMemoryUserEntityFrameworkStorage();
        }
        else
        {
            return services.AddSqlServerUserEntityFrameworkStorage(connectionString);
        }
    }

    /// <summary>
    /// Registers Entity Framework storage services with SQL Server
    /// </summary>
    public static IServiceCollection AddSqlServerUserEntityFrameworkStorage(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);

                sqlOptions.CommandTimeout(30);
            });

#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
#endif
        });

        services.AddScoped<IStorage<UserRef, User>, EntityFrameworkUserStorage>();
        services.AddScoped<IStorage<UserRef, UserNotification>, EntityFrameworkUserNotificationStorage>();
        services.AddScoped<IStorage<UserRef, UserStatusChange>, EntityFrameworkUserStatusChangeStorage>();

        return services;
    }

    /// <summary>
    /// Registers Entity Framework storage services with in-memory database (for testing)
    /// </summary>
    public static IServiceCollection AddInMemoryUserEntityFrameworkStorage(
        this IServiceCollection services,
        string databaseName = "UserDb")
    {
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseInMemoryDatabase(databaseName);

#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
#endif
        });

        services.AddScoped<IStorage<UserRef, User>, EntityFrameworkUserStorage>();
        services.AddScoped<IStorage<UserRef, UserNotification>, EntityFrameworkUserNotificationStorage>();
        services.AddScoped<IStorage<UserRef, UserStatusChange>, EntityFrameworkUserStatusChangeStorage>();

        return services;
    }

    /// <summary>
    /// Ensures the database is created
    /// </summary>
    public static async Task EnsureDatabaseCreatedAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
        await context.Database.EnsureCreatedAsync().ConfigureAwait(false);
    }
}
