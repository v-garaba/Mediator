using Mediators.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Mediators.Repository;

public static class RegistrationExtensions
{
    public static IServiceCollection RegisterRepositories(this IServiceCollection services)
    {
        services
            .AddSingleton<IStorage<MessageRef, ChatMessage>, MessageStorage>()
            .AddSingleton<IStorage<UserRef, User>, UserStorage>()
            .AddSingleton<IStorage<UserRef, UserNotification>, UserNotificationStorage>()
            .AddSingleton<IStorage<UserRef, UserStatusChange>, UserStatusChangeStorage>();

        return services;
    }
}
