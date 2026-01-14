using Microsoft.Extensions.DependencyInjection;

namespace Mediators.RequestHandlers;

public static class RegistrationExtensions
{
    public static IServiceCollection RegisterRequestHandlers(this IServiceCollection services)
    {
        services
            .AddTransient<IRequestHandler, GetAllMessagesHandler>()
            .AddTransient<IRequestHandler, GetMessagesByUserHandler>()
            .AddTransient<IRequestHandler, GetMessageCountHandler>()
            .AddTransient<IRequestHandler, GetUserHandler>();

        return services;
    }
}
