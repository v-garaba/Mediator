﻿using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Mediators.NotificationHandlers;

// NOTE: Broken down from SendMessageNotification
public sealed record HandlePublicMessageNotification(
    ChatMessage Message
) : INotification;

public sealed class HandlePublicMessageNotificationHandler(
    IServiceProvider serviceProvider,
    IStorage<UserRef, User> userStorage)
    : INotificationHandler<HandlePublicMessageNotification>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider.AssertNotNull();
    private readonly IStorage<UserRef, User> _userStorage = userStorage.AssertNotNull();

    public async Task HandleAsync(HandlePublicMessageNotification notification)
    {
        if (notification.Message.Type != MessageType.Public)
        {
            throw new InvalidOperationException("Notification type must be Public for HandlePublicMessageNotification");
        }

        var users = await _userStorage.GetAllAsync();
        foreach (var user in users)
        {
            if (user.Id != notification.Message.SenderId)
            {
                // Lazy resolve mediator to avoid circular dependency
                var mediator = _serviceProvider.GetRequiredService<IMediator>();
                await mediator.PublishAsync(new NotifyUserOfMessageNotification(user, notification.Message));
            }
        }
    }
}
