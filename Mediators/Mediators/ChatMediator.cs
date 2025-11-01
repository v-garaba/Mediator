using System.Collections.Immutable;
using Mediators.NotificationHandlers;
using Mediators.RequestHandlers;

namespace Mediators.Mediators;

public sealed class ChatMediator(
    IEnumerable<IRequestHandler> requestHandlers,
    IEnumerable<INotificationHandler> notificationHandlers)
    : IMediator
{
    private readonly Lazy<IRequestHandler[]> _requestHandlers = new(() => [.. requestHandlers]);
    private readonly Lazy<INotificationHandler[]> _notificationHandlers = new(() => [.. notificationHandlers]);

    #region INotificationObserver Implementation
    public async Task PublishAsync(INotification notification)
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

    public void Subscribe<TNotification>(Func<TNotification, Task> handler) // TO DELETE
        where TNotification : INotification
    {
    }
    #endregion

    #region IRequestObserver Implementation
    public async Task<TResponse> SendRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        where TResponse : class
    {
        request.AssertNotNull();

        // Find the handler that can handle this specific request type
        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        var requestHandlers = _requestHandlers.Value
            .Where(h => handlerType.IsAssignableFrom(h.GetType()))
            .ToImmutableArray();

        if (requestHandlers.Length > 1)
        {
            throw new InvalidOperationException(
                $"Multiple handlers registered for request type {request.GetType().FullName}"
            );
        }

        if (requestHandlers.Length == 0)
        {
            throw new InvalidOperationException(
                $"No handler registered for request type {request.GetType().FullName}"
            );
        }

        // Use reflection to call the Handle method
        var handler = requestHandlers[0];
        var handleMethod = handler.GetType().GetMethod("HandleAsync")
            ?? throw new InvalidOperationException($"Handler {handler.GetType().FullName} does not have a HandleAsync method");


        if (handleMethod.Invoke(handler, [request, CancellationToken.None]) is not Task<TResponse> task)
        {
            throw new InvalidOperationException($"Failed to invoke HandleAsync method on handler {handler.GetType().FullName}");
        }

        var response = await task.ConfigureAwait(false);
        return response;
    }
    #endregion
}
