using Mediators.Mediators;
using Mediators.Models;
using Mediators.Notifications;
using Mediators.Repository;
using Mediators.Repository.EntityFramework;
using Mediators.RequestHandlers;
using Mediators.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Mediators.Tests.Integration;

[TestFixture]
internal sealed class HandleMessagesTests
{
    private readonly UserRef _userId = new();
    private MediatorBasedChatRoom _chatRoom = null!;
    private ServiceProvider _serviceProvider = null!;

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = new ServiceCollection()
            .AddLogging(configure => configure.AddConsole().SetMinimumLevel(LogLevel.Information))
            // Register repositories (in-memory for users, EF for messages)
            .RegisterUserRepositories(MockConfiguration.Default)
            .AddInMemoryEntityFrameworkStorage($"TestDb_{Guid.NewGuid()}") // Unique in-memory DB per test
            // Register handlers
            .RegisterChatRequestHandlers()
            .RegisterUserRequestHandlers()
            .RegisterChatNotificationHandlers()
            .RegisterUserNotificationHandlers()
            // Register mediator
            .AddSingleton<IMediator, ChatMediator>()
            .AddSingleton<MediatorBasedChatRoom>()
            .BuildServiceProvider();

        _chatRoom = _serviceProvider.GetRequiredService<MediatorBasedChatRoom>();
    }

    [TearDown]
    public void TearDown()
    {
        _serviceProvider.Dispose();
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

/// <summary>
/// A chat room that uses the mediator directly (not HTTP).
/// Used for integration testing with Entity Framework.
/// </summary>
internal sealed class MediatorBasedChatRoom(IMediator mediator, ILogger<MediatorBasedChatRoom> logger)
{
    private readonly ILogger<MediatorBasedChatRoom> _logger = logger;
    private readonly IMediator _mediator = mediator;

    public async Task SendMessageAsync(
        UserRef senderId,
        string content,
        MessageType type,
        UserRef? targetUserId = null
    )
    {
        _logger.LogInformation(
            "[CHAT ROOM] User {UserId} is sending a {Kind} message",
            senderId,
            type == MessageType.Private ? "private" : "public"
        );
        await _mediator.PublishAsync(new SendMessageNotification(senderId, content, type, targetUserId));
    }

    public async Task AddUserAsync(User user)
    {
        _logger.LogInformation("[CHAT ROOM] User {User} is joining the chat room", user.Name);
        await _mediator.PublishAsync(new AddUserNotification(user));
    }

    public async Task ChangeUserStatusAsync(UserRef userId, UserStatus newStatus)
    {
        _logger.LogInformation("[CHAT ROOM] User {UserId} is changing status to {Status}", userId, newStatus);
        await _mediator.PublishAsync(new ChangeUserStatusNotification(userId, newStatus));
    }
}
