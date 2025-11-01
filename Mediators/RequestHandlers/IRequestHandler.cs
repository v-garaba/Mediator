namespace Mediators.RequestHandlers;

public interface IRequestHandler { }

public interface IRequestHandler<in TRequest, TResponse> : IRequestHandler
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
}
