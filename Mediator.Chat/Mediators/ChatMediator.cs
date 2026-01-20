using System.Collections.Frozen;
using System.Collections.Immutable;
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
        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);

        var notificationHandlers = _notificationHandlers.Value
            .Where(h => handlerType.IsAssignableFrom(h.GetType()))
            .ToImmutableArray();

        foreach (var handler in notificationHandlers)
        {
            var handleMethod = handler.GetType().GetMethod("HandleAsync")
            ?? throw new InvalidOperationException($"Handler {handler.GetType().FullName} does not have a HandleAsync method");

            if (handleMethod.Invoke(handler, [notification]) is not Task task)
            {
                throw new InvalidOperationException($"Failed to invoke Handle method on handler {handler.GetType().FullName}");
            }

            await task.ConfigureAwait(false);
        }
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
            if (handler is IRequestHandler<IRequest<TResponse>, TResponse> typedHandler)
            {
                return await typedHandler.HandleAsync(request, cancellationToken).ConfigureAwait(false);
            }
        }

        throw new InvalidOperationException(
                $"No handler registered for request type {request.GetType().FullName}");
    }
    #endregion
}


