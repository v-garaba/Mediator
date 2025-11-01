using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using NUnit.Framework;

namespace Mediators.RequestHandlers.Tests;

[TestFixture]
internal sealed class GetMessageCountHandlerTests
{
    [Test]
    public async Task GetMessageCount_ReturnsZeroForNewUser()
    {
        // Arrange
        var userId = new UserRef();
        IStorage<MessageRef, ChatMessage> messageStorage = new MessageStorage();
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

        IStorage<MessageRef, ChatMessage> messageStorage = new MessageStorage();
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
