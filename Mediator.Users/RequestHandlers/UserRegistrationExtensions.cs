using Microsoft.Extensions.DependencyInjection;

namespace Mediators.RequestHandlers;

public static class UserRegistrationExtensions
{
    public static IServiceCollection RegisterUserRequestHandlers(this IServiceCollection services)
    {
        services
            .AddTransient<IRequestHandler, GetUserHandler>();

        return services;
    }
}


