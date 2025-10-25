namespace Mediators.Messaging.Requests;

public interface IRequestsObserver
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request)
        where TResponse : class;

    void RegisterHandler<TRequest, TResponse>(Func<TRequest, Task<TResponse>> handler)
        where TRequest : IRequest<TResponse>
        where TResponse : class;
}
