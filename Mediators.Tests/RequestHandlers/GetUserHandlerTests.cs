using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using NUnit.Framework;

namespace Mediators.RequestHandlers.Tests;

[TestFixture]
internal sealed class GetUserHandlerTests
{
    private readonly UserRef _userId = new();

    [Test]
    public async Task GetUser_ReturnsNullForNonExistentUser()
    {
        // Arrange

        var storage = new UserStorage();

        var handler = new GetUserHandler(storage);
        var mediator = new ChatMediator([handler], []);

        // Act
        var resp = await mediator.SendRequestAsync(new GetUserRequest(_userId));

        // Assert
        Assert.That(resp.User, Is.Null);
    }

    [Test]
    public async Task GetUser_ReturnsExistingUser()
    {
        // Arrange
        var user = new User(_userId, "Bob", "bob@example.com", DateTimeOffset.UtcNow, UserStatus.Online);
        IStorage<UserRef, User> storage = new UserStorage();
        await storage.SetAsync(user);

        var handler = new GetUserHandler(storage);
        var mediator = new ChatMediator([handler], []);

        // Act
        var resp = await mediator.SendRequestAsync(new GetUserRequest(_userId));

        // Assert
        Assert.That(resp.User, Is.Not.Null);
        Assert.That(resp.User, Is.EqualTo(user));
    }
}
