using Mediators.Messaging.Notifications;
using Mediators.Messaging.Requests;

namespace Mediators.Messaging;

public sealed class ChatMediator : IMediator
{
    #region INotificationObserver Implementation
    private readonly Dictionary<Type, List<Func<INotification, Task>>> _subscribers = [];

    public async Task Publish(INotification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);

        if (_subscribers.TryGetValue(notification.GetType(), out var actions))
        {
            foreach (var action in actions)
            {
                await action(notification).ConfigureAwait(false);
            }
        }
    }

    public void Subscribe<TNotification>(Func<TNotification, Task> handler)
        where TNotification : INotification
    {
        if (!_subscribers.TryGetValue(typeof(TNotification), out var actions))
        {
            actions = [];
            _subscribers[typeof(TNotification)] = actions;
        }

        actions.Add(async msg => await handler((TNotification)msg).ConfigureAwait(false));
    }
    #endregion

    #region IRequestObserver Implementation
    private readonly Dictionary<Type, List<Func<IRequest, Task<object>>>> _requestHandlers = [];

    public void RegisterHandler<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler)
        where TRequest : IRequest<TResponse>
        where TResponse : class
    {
        if (!_requestHandlers.ContainsKey(typeof(TRequest)))
        {
            _requestHandlers[typeof(TRequest)] = [];
        }

        _requestHandlers[typeof(TRequest)]
            .Add(async req => await handler((TRequest)req).ConfigureAwait(false));
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        where TResponse : class
    {
        ArgumentNullException.ThrowIfNull(request);

        if (_requestHandlers.TryGetValue(request.GetType(), out var handlers))
        {
            if (handlers.Count > 0)
            {
                // For simplicity, we only invoke the first registered handler
                if (await handlers[0](request) is not TResponse result)
                {
                    throw new InvalidOperationException(
                        $"Handler for request type {request.GetType().FullName} did not return a response"
                    );
                }

                return result;
            }
        }

        throw new InvalidOperationException(
            $"No handler registered for request type {request.GetType().FullName}"
        );
    }
    #endregion
}
