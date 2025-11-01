using Mediators.Models;
using Mediators.Repository;
using Mediators.Repository.EntityFramework;
using Mediators.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Mediators.Tests.Repository;

[TestFixture]
public class EntityFrameworkMessageStorageTests
{
    private IServiceProvider _serviceProvider = null!;
    private IStorage<MessageRef, ChatMessage> _storage = null!;

    [SetUp]
    public void Setup()
    {
        // Create a new service provider with unique in-memory database for each test
        var services = new ServiceCollection();
        services.AddInMemoryEntityFrameworkStorage($"TestDb_{Guid.NewGuid()}");
        _serviceProvider = services.BuildServiceProvider();
        _storage = _serviceProvider.GetRequiredService<IStorage<MessageRef, ChatMessage>>();
    }

    [TearDown]
    public async Task TearDown()
    {
        // Clean up the database after each test
        await _serviceProvider.WipeInMemoryDatabaseAsync();
        
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    [Test]
    public async Task SetAsync_StoresMessage()
    {
        // Arrange
        var message = new ChatMessage(
            new UserRef(),
            "Test message",
            MessageType.Public
        );

        // Act
        await _storage.SetAsync(message);

        // Assert
        var count = await _serviceProvider.GetMessageCountAsync();
        Assert.That(count, Is.EqualTo(1));
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllMessages()
    {
        // Arrange
        var message1 = new ChatMessage(new UserRef(), "Message 1", MessageType.Public);
        var message2 = new ChatMessage(new UserRef(), "Message 2", MessageType.Public);
        await _storage.SetAsync(message1);
        await _storage.SetAsync(message2);

        // Act
        var messages = await _storage.GetAllAsync();

        // Assert
        Assert.That(messages.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task TryGetAsync_ReturnsMessage_WhenExists()
    {
        // Arrange
        var message = new ChatMessage(new UserRef(), "Test", MessageType.Public);
        await _storage.SetAsync(message);

        // Act
        var retrieved = await _storage.TryGetAsync(message.Id);

        // Assert
        Assert.That(retrieved, Is.Not.Null);
        Assert.That(retrieved!.Content, Is.EqualTo("Test"));
    }

    [Test]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        // Arrange
        await _storage.SetAsync(new ChatMessage(new UserRef(), "1", MessageType.Public));
        await _storage.SetAsync(new ChatMessage(new UserRef(), "2", MessageType.Public));
        await _storage.SetAsync(new ChatMessage(new UserRef(), "3", MessageType.Public));

        // Act
        var count = await _storage.CountAsync();

        // Assert
        Assert.That(count, Is.EqualTo(3));
    }

    [Test]
    public async Task ClearMessages_RemovesAllMessages()
    {
        // Arrange
        await _storage.SetAsync(new ChatMessage(new UserRef(), "1", MessageType.Public));
        await _storage.SetAsync(new ChatMessage(new UserRef(), "2", MessageType.Public));
        
        // Act
        await _serviceProvider.ClearMessagesAsync();

        // Assert
        var count = await _storage.CountAsync();
        Assert.That(count, Is.EqualTo(0));
    }
}
