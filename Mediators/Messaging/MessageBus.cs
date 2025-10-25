using Mediators.Services;

namespace Mediators.Messaging;

public sealed class MessageBus
{
    private readonly Dictionary<Type, List<Action<IRequest>>> _subscribers = [];

    public void Publish(IRequest message)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (_subscribers.TryGetValue(message.GetType(), out var actions))
        {
            foreach (var action in actions)
            {
                action(message);
            }
        }
    }

    public void Subscribe<TRequest>(Action<TRequest> handler)
        where TRequest : IRequest
    {
        if (!_subscribers.TryGetValue(typeof(TRequest), out var actions))
        {
            actions = [];
            _subscribers[typeof(TRequest)] = actions;
        }

        actions.Add(msg => handler((TRequest)msg));
    }
}
