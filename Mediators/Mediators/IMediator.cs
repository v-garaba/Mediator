using Mediators.NotificationHandlers;
using Mediators.RequestHandlers;

namespace Mediators.Mediators;

public interface IMediator
{
    /// <summary>
    /// Sends a request to the appropriate handler and returns the response.
    /// </summary>
    Task<TResponse> SendRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        where TResponse : class;

    /// <summary>
    /// Publishes a notification to all appropriate handlers.
    /// </summary>
    Task PublishAsync(INotification notification);
}
