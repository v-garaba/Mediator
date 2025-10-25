namespace Mediators.Messaging;

public sealed record SendSmsRequest(string UserId, string Message) : IRequest;
