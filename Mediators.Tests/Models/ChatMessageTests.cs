using Mediators.Models;
using NUnit.Framework;

namespace Mediators.Tests.Models;

public class ChatMessageTests
{
    [Test]
    public void ChatMessage_Creation_ShouldSetAllProperties()
    {
        // Arrange & Act
        var message = new ChatMessage("1", "Hello World", MessageType.Public);

        // Assert
        Assert.That(message.Id, Is.Not.Null);
        Assert.That(message.SenderId, Is.EqualTo("1"));
        Assert.That(message.Content, Is.EqualTo("Hello World"));
        Assert.That(message.Type, Is.EqualTo(MessageType.Public));
        Assert.That(message.TargetUserId, Is.Null);
        Assert.That(message.Timestamp, Is.LessThanOrEqualTo(DateTime.UtcNow));
    }

    [Test]
    public void ChatMessage_Private_ShouldHaveTargetUser()
    {
        // Arrange & Act
        var message = new ChatMessage("1", "Private msg", MessageType.Private, "2");

        // Assert
        Assert.That(message.Type, Is.EqualTo(MessageType.Private));
        Assert.That(message.TargetUserId, Is.EqualTo("2"));
    }

    [Test]
    public void ChatMessage_GeneratesUniqueIds()
    {
        // Arrange & Act
        var message1 = new ChatMessage("1", "Message 1", MessageType.Public);
        var message2 = new ChatMessage("1", "Message 2", MessageType.Public);

        // Assert
        Assert.That(message1.Id, Is.Not.EqualTo(message2.Id));
    }

    [Test]
    public void ChatMessage_TimestampIsReasonable()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var message = new ChatMessage("1", "Test", MessageType.Public);

        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.That(message.Timestamp, Is.InRange(beforeCreation, afterCreation));
    }
}
