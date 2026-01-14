using Mediators.NotificationHandlers;
using Mediators.RequestHandlers;

namespace Mediators.Mediators;

public interface IMediator
{
    /// <summary>
    /// Sends a request to the appropriate handler and returns the response.
    /// </summary>
    Task<TResponse> SendRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        where TResponse : notnull;

    /// <summary>
    /// Publishes a notification to all appropriate handlers.
    /// </summary>
    Task PublishAsync<TNotification>(TNotification notification)
        where TNotification : INotification;
}
