using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using Mediators.Repository.EntityFramework;
using Mediators.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Mediators.RequestHandlers.Tests;

[TestFixture]
internal sealed class GetMessageCountHandlerTests
{
    private ServiceProvider _serviceProvider = null!;

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = new ServiceCollection()
            .RegisterChatRepositories(MockConfiguration.Default)
            .AddInMemoryEntityFrameworkStorage($"TestDb_{Guid.NewGuid()}") // Unique in-memory DB per test
            .BuildServiceProvider();
    }

    [TearDown]
    public void TearDown()
    {
        _serviceProvider.Dispose();
    }

    [Test]
    public async Task GetMessageCount_ReturnsZeroForNewUser()
    {
        // Arrange
        var userId = new UserRef();
        IStorage<MessageRef, ChatMessage> messageStorage = _serviceProvider.GetRequiredService<IStorage<MessageRef, ChatMessage>>();
        var handler = new GetMessageCountHandler(messageStorage);
        var mediator = new ChatMediator([handler], []);

        // Act
        var resp = await mediator.SendRequestAsync(new GetMessageCountRequest(userId));

        // Assert
        Assert.That(resp.Count, Is.EqualTo(0));
    }

    [Test]
    public async Task GetMessageCount_ReturnsExpected()
    {
        // Arrange
        var userId = new UserRef();

        IStorage<MessageRef, ChatMessage> messageStorage = _serviceProvider.GetRequiredService<IStorage<MessageRef, ChatMessage>>();
        await messageStorage.SetAsync(new ChatMessage(userId, "Hello", MessageType.Public));
        await messageStorage.SetAsync(new ChatMessage(userId, "World", MessageType.Public));

        var handler = new GetMessageCountHandler(messageStorage);
        var mediator = new ChatMediator([handler], []);

        // Act
        var resp = await mediator.SendRequestAsync(new GetMessageCountRequest(userId));

        // Assert
        Assert.That(resp.Count, Is.EqualTo(2));
    }
}


