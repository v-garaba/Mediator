namespace Mediators.Messaging.Requests;

public interface IRequest { }

public interface IRequest<TResponse> : IRequest
    where TResponse : class { }
