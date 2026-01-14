using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using NUnit.Framework;

namespace Mediators.RequestHandlers.Tests;

[TestFixture]
internal sealed class GetAllMessagesHandlerTests
{
    private readonly UserRef _userId = new();

    [Test]
    public async Task StoreMultipleMessages_MaintainsOrder()
    {
        // Arrange
        var message1 = new ChatMessage(_userId, "First", MessageType.Public);
        var message2 = new ChatMessage(_userId, "Second", MessageType.Public);
        var message3 = new ChatMessage(_userId, "Third", MessageType.Public);

        IStorage<MessageRef, ChatMessage> messageStorage = new MessageStorage();
        await messageStorage.SetAsync(message1);
        await messageStorage.SetAsync(message2);
        await messageStorage.SetAsync(message3);

        var handler = new GetAllMessagesHandler(messageStorage);
        var mediator = new ChatMediator([handler], []);

        // Act
        var resp = await mediator.SendRequestAsync(new GetAllMessagesRequest());

        // Assert
        Assert.That(resp.Messages.Count, Is.EqualTo(3));
        Assert.That(resp.Messages[0].Content, Is.EqualTo("First"));
        Assert.That(resp.Messages[1].Content, Is.EqualTo("Second"));
        Assert.That(resp.Messages[2].Content, Is.EqualTo("Third"));
    }
}
