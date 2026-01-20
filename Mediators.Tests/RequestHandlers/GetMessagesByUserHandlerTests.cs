using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using Mediators.Repository.EntityFramework;
using Mediators.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Mediators.RequestHandlers.Tests;

[TestFixture]
internal sealed class GetMessagesByUserHandlerTests
{
    private readonly UserRef _userId = new();
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
    public async Task StoreMultipleMessages_MaintainsOrder()
    {
        // Arrange
        var user1 = _userId;
        var user2 = new UserRef();
        var message1 = new ChatMessage(user1, "First", MessageType.Public);
        var message2 = new ChatMessage(user2, "Second", MessageType.Public);
        var message3 = new ChatMessage(user1, "Third", MessageType.Public);

        IStorage<MessageRef, ChatMessage> messageStorage = _serviceProvider.GetRequiredService<
            IStorage<MessageRef, ChatMessage>
        >();
        await messageStorage.SetAsync(message1);
        await messageStorage.SetAsync(message2);
        await messageStorage.SetAsync(message3);

        var handler = new GetMessagesByUserHandler(messageStorage);
        var mediator = new ChatMediator([handler], []);

        // Act
        var resp_user1 = await mediator.SendRequestAsync(new GetMessagesByUserRequest(user1));

        // Assert
        Assert.That(resp_user1.Messages.Count, Is.EqualTo(2));
        Assert.That(resp_user1.Messages[0].Content, Is.EqualTo("First"));
        Assert.That(resp_user1.Messages[1].Content, Is.EqualTo("Third"));

        // Act
        var resp_user2 = await mediator.SendRequestAsync(new GetMessagesByUserRequest(user2));

        // Assert
        Assert.That(resp_user2.Messages.Count, Is.EqualTo(1));
        Assert.That(resp_user2.Messages[0].Content, Is.EqualTo("Second"));
    }
}


