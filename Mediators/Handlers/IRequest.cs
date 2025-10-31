namespace Mediators.Handlers;

public interface IRequest { }

public interface IRequest<TResponse> : IRequest
    where TResponse : class { }
