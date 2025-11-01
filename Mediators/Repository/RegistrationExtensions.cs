using Mediators.Models;
using Mediators.Repository.EntityFramework;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mediators.Repository;

public static class RegistrationExtensions
{
    public static IServiceCollection RegisterRepositories(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var useInMemoryDb = configuration.GetValue<bool>("UseInMemoryDatabase");
        var connectionString = configuration.GetConnectionString("ChatDatabase")
            ?? throw new InvalidOperationException("ChatDatabase connection string is not configured");

        services
            .AddEntityFrameworkStorage(useInMemoryDb, connectionString) // Register EF storage
            .AddSingleton<IStorage<MessageRef, ChatMessage>, EntityFrameworkMessageStorage>()
            .AddSingleton<IStorage<UserRef, User>, UserStorage>()
            .AddSingleton<IStorage<UserRef, UserNotification>, UserNotificationStorage>()
            .AddSingleton<IStorage<UserRef, UserStatusChange>, UserStatusChangeStorage>();

        return services;
    }
}
