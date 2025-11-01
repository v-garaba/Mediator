using Microsoft.Extensions.DependencyInjection;

namespace Mediators.NotificationHandlers;

public static class RegistrationExtensions
{
    public static IServiceCollection RegisterNotificationHandlers(this IServiceCollection services)
    {
        services
            .AddTransient<INotificationHandler, AddUserNotificationHandler>()
            .AddTransient<INotificationHandler, ChangeUserStatusNotificationHandler>()
            .AddTransient<INotificationHandler, EmailNotificationHandler>()
            .AddTransient<INotificationHandler, HandlePublicMessageNotificationHandler>()
            .AddTransient<INotificationHandler, HandlePrivateMessageNotificationHandler>()
            .AddTransient<INotificationHandler, NotifyUserOfMessageNotificationHandler>()
            .AddTransient<INotificationHandler, NotifyUserStatusChangeNotificationHandler>()
            .AddTransient<INotificationHandler, RegisterUserNotificationHandler>()
            .AddTransient<INotificationHandler, SendMessageNotificationHandler>()
            .AddTransient<INotificationHandler, SendPushNotificationNotificationHandler>()
            .AddTransient<INotificationHandler, SendSmsNotificationHandler>()
            .AddTransient<INotificationHandler, StoreMessageNotificationHandler>()
            .AddTransient<INotificationHandler, TrackMessageNotificationHandler>()
            .AddTransient<INotificationHandler, TrackMessageSentNotificationHandler>()
            .AddTransient<INotificationHandler, TrackUserStatusChangeNotificationHandler>()
            .AddTransient<INotificationHandler, UpdateUserActivityNotificationHandler>()
            .AddTransient<INotificationHandler, UpdateUserStatusNotificationHandler>();

        return services;
    }
}
