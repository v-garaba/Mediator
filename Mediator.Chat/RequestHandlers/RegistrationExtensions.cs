using Microsoft.Extensions.DependencyInjection;

namespace Mediators.RequestHandlers;

public static class RegistrationExtensions
{
    public static IServiceCollection RegisterChatRequestHandlers(this IServiceCollection services)
    {
        services
            .AddTransient<IRequestHandler, GetAllMessagesHandler>()
            .AddTransient<IRequestHandler, GetMessagesByUserHandler>()
            .AddTransient<IRequestHandler, GetMessageCountHandler>();

        return services;
    }
}


