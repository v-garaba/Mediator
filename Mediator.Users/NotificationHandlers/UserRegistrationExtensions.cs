using Microsoft.Extensions.DependencyInjection;

namespace Mediators.Notifications;

public static class UserRegistrationExtensions
{
    public static IServiceCollection RegisterUserNotificationHandlers(this IServiceCollection services)
    {
        services
            .AddTransient<INotificationHandler, ChangeUserStatusNotificationHandler>()
            .AddTransient<INotificationHandler, EmailNotificationHandler>()
            .AddTransient<INotificationHandler, SendPushNotificationNotificationHandler>()
            .AddTransient<INotificationHandler, SendSmsNotificationHandler>()
            .AddTransient<INotificationHandler, NotifyUserStatusChangeNotificationHandler>()
            .AddTransient<INotificationHandler, RegisterUserNotificationHandler>()
            .AddTransient<INotificationHandler, TrackUserStatusChangeNotificationHandler>()
            .AddTransient<INotificationHandler, UpdateUserActivityNotificationHandler>()
            .AddTransient<INotificationHandler, UpdateUserStatusNotificationHandler>();

        return services;
    }
}


