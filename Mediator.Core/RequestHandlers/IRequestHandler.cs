namespace Mediators.RequestHandlers;

public interface IRequestHandler
{
    Type RequestType { get; }
}

public interface IRequestHandler<TRequest, TResponse> : IRequestHandler
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
}


