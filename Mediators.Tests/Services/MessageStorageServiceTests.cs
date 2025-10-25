using Mediators.Models;
using Mediators.Services;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Mediators.Tests.Services;

public class MessageStorageServiceTests
{
    private MessageStorageService _storageService = null!;

    [SetUp]
    public void Setup()
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.SetMinimumLevel(LogLevel.None));
        var logger = loggerFactory.CreateLogger<MessageStorageService>();
        _storageService = new MessageStorageService(logger);
    }

    [Test]
    public void StoreMessage_AddsMessageToStorage()
    {
        // Arrange
        var message = new ChatMessage("1", "Test message", MessageType.Public);

        // Act
        _storageService.StoreMessage(message);
        var messages = _storageService.GetAllMessages();

        // Assert
        Assert.That(messages, Has.Count.EqualTo(1));
        Assert.That(messages, Has.Some.Matches<ChatMessage>(m => m.Id == message.Id));
    }

    [Test]
    public void GetAllMessages_InitiallyEmpty()
    {
        // Arrange & Act
        var messages = _storageService.GetAllMessages();

        // Assert
        Assert.That(messages, Is.Empty);
    }

    [Test]
    public void GetMessagesByUser_FiltersCorrectly()
    {
        // Arrange
        var user1Message = new ChatMessage("user1", "Message from user1", MessageType.Public);
        var user2Message = new ChatMessage("user2", "Message from user2", MessageType.Public);
        var anotherUser1Message = new ChatMessage(
            "user1",
            "Another from user1",
            MessageType.Public
        );

        _storageService.StoreMessage(user1Message);
        _storageService.StoreMessage(user2Message);
        _storageService.StoreMessage(anotherUser1Message);

        // Act
        var user1Messages = _storageService.GetMessagesByUser("user1");

        // Assert
        Assert.That(user1Messages, Has.Count.EqualTo(2));
        Assert.That(user1Messages, Has.All.Matches<ChatMessage>(m => m.SenderId == "user1"));
    }

    [Test]
    public void StoreMultipleMessages_MaintainsOrder()
    {
        // Arrange
        var message1 = new ChatMessage("1", "First", MessageType.Public);
        var message2 = new ChatMessage("1", "Second", MessageType.Public);
        var message3 = new ChatMessage("1", "Third", MessageType.Public);

        // Act
        _storageService.StoreMessage(message1);
        _storageService.StoreMessage(message2);
        _storageService.StoreMessage(message3);

        var messages = _storageService.GetAllMessages();

        // Assert
        Assert.That(messages, Has.Count.EqualTo(3));
        Assert.That(messages[0].Content, Is.EqualTo("First"));
        Assert.That(messages[1].Content, Is.EqualTo("Second"));
        Assert.That(messages[2].Content, Is.EqualTo("Third"));
    }
}
