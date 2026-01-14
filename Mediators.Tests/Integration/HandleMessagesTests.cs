using Mediators.Clients;
using Mediators.Mediators;
using Mediators.Models;
using Mediators.NotificationHandlers;
using Mediators.Repository;
using Mediators.RequestHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Mediators.Tests.Integration;

[TestFixture]
internal sealed class HandleMessagesTests
{
    private readonly UserRef _userId = new();
    private ChatRoom _chatRoom = null!;
    private ServiceProvider _serviceProvider = null!;

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = new ServiceCollection()
            .AddLogging(configure => configure.AddConsole().SetMinimumLevel(LogLevel.Information))
            .RegisterRepositories()
            .RegisterRequestHandlers()
            .RegisterNotificationHandlers()
            .AddSingleton<IMediator, ChatMediator>()
            .AddSingleton<ChatRoom>()
            .BuildServiceProvider();

        _chatRoom = _serviceProvider.GetRequiredService<ChatRoom>();
    }

    [Test]
    public async Task SendMessage_EnsureAllHandlersTrigger()
    {
        // Arrange
        IStorage<UserRef, User> _userStorage = _serviceProvider.GetRequiredService<IStorage<UserRef, User>>();
        var user = new User(_userId, "Bob", "bob@example.com", DateTimeOffset.UtcNow, UserStatus.Online);
        var otherUser = new User(new UserRef(), "Alice", "alice@example.com", DateTimeOffset.UtcNow, UserStatus.Online);
        await _userStorage.SetAsync(user);
        await _userStorage.SetAsync(otherUser);

        // Act
        DateTimeOffset beforeSendTime = DateTimeOffset.UtcNow;
        await _chatRoom.SendMessageAsync(_userId, "Some content", MessageType.Public);

        // Assert - Message handler
        IStorage<MessageRef, ChatMessage> _messageStorage = _serviceProvider.GetRequiredService<IStorage<MessageRef, ChatMessage>>();
        var messagesResp = await _messageStorage.GetAllAsync();

        Assert.That(messagesResp, Has.Length.EqualTo(1));
        Assert.That(messagesResp[0].SenderId, Is.EqualTo(_userId));
        Assert.That(messagesResp[0].Content, Is.EqualTo("Some content"));
        Assert.That(messagesResp[0].Type, Is.EqualTo(MessageType.Public));
        Assert.That(messagesResp[0].TargetUserId, Is.Null);

        // Assert - Other users notified
        var _userNotificationsStorage = _serviceProvider.GetRequiredService<IStorage<UserRef, UserNotification>>();
        var otherUserNotifications = await _userNotificationsStorage.TryGetAsync(otherUser.Id);
        Assert.That(otherUserNotifications, Is.Not.Null);
        Assert.That(otherUserNotifications!.MessageCount, Is.EqualTo(1));

        // Also ensure our user did not get notified of their own message
        var userNotifications = await _userNotificationsStorage.TryGetAsync(user.Id);
        Assert.That(userNotifications, Is.Null);

        // Assert - User activity updated
        var updatedUser = await _userStorage.TryGetAsync(user.Id);
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser!.LastActiveTime, Is.EqualTo(beforeSendTime).Within(TimeSpan.FromSeconds(1)));

    }
}
