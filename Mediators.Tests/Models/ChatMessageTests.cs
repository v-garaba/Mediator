using NUnit.Framework;

namespace Mediators.Models.Tests;

public class ChatMessageTests
{
    [Test]
    public void ChatMessage_Creation_ShouldSetAllProperties()
    {
        // Arrange & Act
        var user1 = new UserRef();
        var message = new ChatMessage(user1, "Hello World", MessageType.Public);

        // Assert
        Assert.That(message.Id, Is.Not.Null);
        Assert.That(message.SenderId, Is.EqualTo(user1));
        Assert.That(message.Content, Is.EqualTo("Hello World"));
        Assert.That(message.Type, Is.EqualTo(MessageType.Public));
        Assert.That(message.TargetUserId, Is.Null);
        Assert.That(message.Timestamp, Is.LessThanOrEqualTo(DateTimeOffset.UtcNow));
    }

    [Test]
    public void ChatMessage_Private_ShouldHaveTargetUser()
    {
        // Arrange & Act
        var user1 = new UserRef();
        var user2 = new UserRef();

        var message = new ChatMessage(user1, "Private msg", MessageType.Private, user2);

        // Assert
        Assert.That(message.Type, Is.EqualTo(MessageType.Private));
        Assert.That(message.TargetUserId, Is.EqualTo(user2));
    }

    [Test]
    public void ChatMessage_GeneratesUniqueIds()
    {
        // Arrange & Act
        var user1 = new UserRef();
        var message1 = new ChatMessage(user1, "Message 1", MessageType.Public);
        var message2 = new ChatMessage(user1, "Message 2", MessageType.Public);

        // Assert
        Assert.That(message1.Id, Is.Not.EqualTo(message2.Id));
    }

    [Test]
    public void ChatMessage_TimestampIsReasonable()
    {
        // Arrange
        var user1 = new UserRef();
        var beforeCreation = DateTimeOffset.UtcNow;

        // Act
        var message = new ChatMessage(user1, "Test", MessageType.Public);

        var afterCreation = DateTimeOffset.UtcNow;

        // Assert
        Assert.That(message.Timestamp, Is.InRange(beforeCreation, afterCreation));
    }
}


