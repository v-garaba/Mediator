using Mediators.Models;

namespace Mediators.Messaging.Requests;

public sealed record GetUserRequest(string UserId) : IRequest<GetUserResponse>;

public sealed record GetUserResponse(User? User);
