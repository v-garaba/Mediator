namespace Mediators.RequestHandlers;

public interface IRequest { }

public interface IRequest<TResponse> : IRequest
    where TResponse : notnull
{
}