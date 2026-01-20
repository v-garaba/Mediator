using Microsoft.Extensions.DependencyInjection;

namespace Mediators.Notifications;

public static class ChatRegistrationExtensions
{
    public static IServiceCollection RegisterChatNotificationHandlers(this IServiceCollection services)
    {
        services
            .AddTransient<INotificationHandler, AddUserNotificationHandler>()
            .AddTransient<INotificationHandler, HandlePublicMessageNotificationHandler>()
            .AddTransient<INotificationHandler, HandlePrivateMessageNotificationHandler>()
            .AddTransient<INotificationHandler, NotifyUserOfMessageNotificationHandler>()
            .AddTransient<INotificationHandler, SendMessageNotificationHandler>()
            .AddTransient<INotificationHandler, StoreMessageNotificationHandler>()
            .AddTransient<INotificationHandler, TrackMessageNotificationHandler>()
            .AddTransient<INotificationHandler, TrackMessageSentNotificationHandler>();

        return services;
    }
}


