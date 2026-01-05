namespace Mediators.RequestHandlers;

public abstract class AbstractRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    public Type RequestType { get; } = typeof(TRequest);
    public Type ResponseType { get; } = typeof(TResponse);
    public abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}
