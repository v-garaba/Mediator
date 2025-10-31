using System.Threading.Tasks;
using Mediators.Messaging;
using Mediators.Messaging.Notifications;
using Mediators.Messaging.Requests;
using Mediators.Models;
using Mediators.Services;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Mediators.Tests.Services;

public class MessageStorageServiceTests
{
    private ChatMediator _mediator;

    [SetUp]
    public void Setup()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.None));
        var logger = loggerFactory.CreateLogger<MessageStorageService>();
        _mediator = new ChatMediator();
        _ = new MessageStorageService(_mediator, logger); // Turn the service on
    }

    [Test]
    public async Task StoreMessage_AddsMessageToStorage()
    {
        // Arrange
        var user1 = new UserRef();
        var message = new ChatMessage(user1, "Test message", MessageType.Public);

        // Act
        await _mediator.Publish(new StoreMessageNotification(message));
        var response = await _mediator.Send(new GetAllMessagesRequest());

        // Assert
        Assert.That(response.Messages, Has.Count.EqualTo(1));
        Assert.That(response.Messages, Has.Some.Matches<ChatMessage>(m => m.Id == message.Id));
    }

    [Test]
    public async Task GetAllMessages_InitiallyEmpty()
    {
        // Arrange & Act
        var response = await _mediator.Send(new GetAllMessagesRequest());

        // Assert
        Assert.That(response.Messages, Is.Empty);
    }

    [Test]
    public async Task GetMessagesByUser_FiltersCorrectly()
    {
        // Arrange
        var user1 = new UserRef();
        var user2 = new UserRef();

        var user1Message = new ChatMessage(user1, "Message from user1", MessageType.Public);
        var user2Message = new ChatMessage(user2, "Message from user2", MessageType.Public);
        var anotherUser1Message = new ChatMessage(
            user1,
            "Another from user1",
            MessageType.Public
        );

        await _mediator.Publish(new StoreMessageNotification(user1Message));
        await _mediator.Publish(new StoreMessageNotification(user2Message));
        await _mediator.Publish(new StoreMessageNotification(anotherUser1Message));

        // Act
        var response1 = await _mediator.Send(new GetMessagesByUserRequest(user1));

        // Assert
        Assert.That(response1.Messages, Has.Count.EqualTo(2));
        Assert.That(response1.Messages, Has.All.Matches<ChatMessage>(m => m.SenderId == user1));
    }

    [Test]
    public async Task StoreMultipleMessages_MaintainsOrder()
    {
        // Arrange
        var user1 = new UserRef();
        var message1 = new ChatMessage(user1, "First", MessageType.Public);
        var message2 = new ChatMessage(user1, "Second", MessageType.Public);
        var message3 = new ChatMessage(user1, "Third", MessageType.Public);

        // Act
        await _mediator.Publish(new StoreMessageNotification(message1));
        await _mediator.Publish(new StoreMessageNotification(message2));
        await _mediator.Publish(new StoreMessageNotification(message3));

        var response = await _mediator.Send(new GetAllMessagesRequest());

        // Assert
        Assert.That(response.Messages, Has.Count.EqualTo(3));
        Assert.That(response.Messages[0].Content, Is.EqualTo("First"));
        Assert.That(response.Messages[1].Content, Is.EqualTo("Second"));
        Assert.That(response.Messages[2].Content, Is.EqualTo("Third"));
    }
}
