using System.Collections.Frozen;
using Mediators.Notifications;
using Mediators.RequestHandlers;

namespace Mediators.Mediators;

public sealed class ChatMediator(
    IEnumerable<IRequestHandler> requestHandlers,
    IEnumerable<INotificationHandler> notificationHandlers)
    : IMediator
{
    private readonly Lazy<FrozenDictionary<Type, IRequestHandler>> _requestHandlers = new Lazy<FrozenDictionary<Type, IRequestHandler>>(requestHandlers.ToFrozenDictionary(x => x.RequestType));
    private readonly Lazy<FrozenDictionary<Type, INotificationHandler>> _notificationHandlers = new Lazy<FrozenDictionary<Type, INotificationHandler>>(notificationHandlers.ToFrozenDictionary(x => x.NotificationType));

    #region INotificationObserver Implementation
    public async Task PublishAsync<TNotification>(TNotification notification)
        where TNotification : INotification
    {
        notification.AssertNotNull();

        // Find the handlers that can handle this specific notification type
        var notificationType = notification.GetType();

        if (_notificationHandlers.Value.TryGetValue(notificationType, out INotificationHandler? handler))
        {
            if (handler is INotificationHandler<TNotification> typedHandler)
            {
                await typedHandler.HandleAsync(notification).ConfigureAwait(false);
                return;
            }

        }

        throw new InvalidOperationException($"Failed to locate a handler for notification : {notificationType.FullName}");
    }

    public void Subscribe<TNotification>(Func<TNotification, Task> handler) // TO DELETE
        where TNotification : INotification
    {
    }
    #endregion

    #region IRequestObserver Implementation
    public async Task<TResponse> SendRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        where TResponse : notnull
    {
        request.AssertNotNull();

        // Find the handler that can handle this specific request type
        var requestType = request.GetType();

        if (_requestHandlers.Value.TryGetValue(requestType, out var handler))
        {
            // Use reflection to invoke HandleAsync since generic variance prevents direct casting
            var handleMethod = handler.GetType().GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.HandleAsync));
            if (handleMethod is not null)
            {
                var task = (Task)handleMethod.Invoke(handler, [request, cancellationToken])!;
                await task.ConfigureAwait(false);
                
                // Get the Result property from Task<TResponse>
                var resultProperty = task.GetType().GetProperty(nameof(Task<TResponse>.Result))!;
                return (TResponse)resultProperty.GetValue(task)!;
            }
        }

        throw new InvalidOperationException(
                $"No handler registered for request type {request.GetType().FullName}");
    }
    #endregion
}


