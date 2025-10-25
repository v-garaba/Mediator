using Mediators.Services;

namespace Mediators.Messaging;

public sealed class MessageBus
{
    private readonly Dictionary<Type, List<Func<IRequest, Task>>> _subscribers = [];

    public async Task Publish(IRequest message)
    {
        ArgumentNullException.ThrowIfNull(message);

        if (_subscribers.TryGetValue(message.GetType(), out var actions))
        {
            foreach (var action in actions)
            {
                await action(message).ConfigureAwait(false);
            }
        }
    }

    public void Subscribe<TRequest>(Func<TRequest, Task> handler)
        where TRequest : IRequest
    {
        if (!_subscribers.TryGetValue(typeof(TRequest), out var actions))
        {
            actions = [];
            _subscribers[typeof(TRequest)] = actions;
        }

        actions.Add(async msg => await handler((TRequest)msg).ConfigureAwait(false));
    }
}
