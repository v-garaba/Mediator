using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using NUnit.Framework;

namespace Mediators.RequestHandlers.Tests;

[TestFixture]
internal sealed class GetMessagesByUserHandlerTests
{
    private readonly UserRef _userId = new();

    [Test]
    public async Task StoreMultipleMessages_MaintainsOrder()
    {
        // Arrange
        var user1 = _userId;
        var user2 = new UserRef();
        var message1 = new ChatMessage(user1, "First", MessageType.Public);
        var message2 = new ChatMessage(user2, "Second", MessageType.Public);
        var message3 = new ChatMessage(user1, "Third", MessageType.Public);

        IStorage<MessageRef, ChatMessage> messageStorage = new MessageStorage();
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
