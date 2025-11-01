using Mediators.Models;
using Mediators.Repository;

namespace Mediators.RequestHandlers;

public sealed record GetUserRequest(UserRef UserId) : IRequest<GetUserResponse>;

public sealed record GetUserResponse(User? User);

public sealed class GetUserHandler(IStorage<UserRef, User> userStorage)
    : IRequestHandler<GetUserRequest, GetUserResponse>
{
    private readonly IStorage<UserRef, User> _userStorage = userStorage;

    public async Task<GetUserResponse> HandleAsync(GetUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _userStorage.TryGetAsync(request.AssertNotNull().UserId, cancellationToken);
        return new GetUserResponse(user);
    }
}
