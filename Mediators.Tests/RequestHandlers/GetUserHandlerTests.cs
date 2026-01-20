using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using Mediators.Repository.EntityFramework;
using Mediators.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Mediators.RequestHandlers.Tests;

[TestFixture]
internal sealed class GetUserHandlerTests
{
    private readonly UserRef _userId = new();

    private ServiceProvider _serviceProvider = null!;

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = new ServiceCollection()
            .RegisterUserRepositories(MockConfiguration.Default)
            .AddInMemoryEntityFrameworkStorage($"TestDb_{Guid.NewGuid()}") // Unique in-memory DB per test
            .BuildServiceProvider();
    }

    [TearDown]
    public void TearDown()
    {
        _serviceProvider.Dispose();
    }

    [Test]
    public async Task GetUser_ReturnsNullForNonExistentUser()
    {
        // Arrange

        var storage = _serviceProvider.GetRequiredService<IStorage<UserRef, User>>();

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
        IStorage<UserRef, User> storage = _serviceProvider.GetRequiredService<IStorage<UserRef, User>>();
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


