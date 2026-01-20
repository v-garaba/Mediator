using Mediators.Mediators;
using Mediators.Models;
using Mediators.Repository;
using Mediators.Repository.EntityFramework;
using Mediators.Tests.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Mediators.Notifications.Tests;

[TestFixture]
internal sealed class RegisterUserNotificationHandlerTests
{
    private readonly UserRef _userRef = new();
private ServiceProvider _serviceProvider = null!;

    [SetUp]
    public void SetUp()
    {
        _serviceProvider = new ServiceCollection()
            .RegisterUserRepositories(MockConfiguration.Default)
            .AddInMemoryEntityFrameworkStorage($"TestDb_{Guid.NewGuid()}") // Unique in-memory DB per test
            .BuildServiceProvider();
    }

    [TearDown]
    public void TearDown()
    {
        _serviceProvider.Dispose();
    }

    [Test]
    public async Task RegisterUser_AddsUserToMemory()
    {
        // Arrange
        var user = new User(_userRef, "Alice", "alice@example.com", DateTimeOffset.UtcNow, UserStatus.Offline);

        IStorage<UserRef, User> userRepository = _serviceProvider.GetRequiredService<IStorage<UserRef, User>>();

        RegisterUserNotificationHandler notificationHandler = new(
            LoggerFactory
                .Create(builder => builder.SetMinimumLevel(LogLevel.None))
                .CreateLogger<RegisterUserNotificationHandler>(),
            userRepository
        );

        var mediator = new ChatMediator([], [notificationHandler]);

        // Act
        await mediator.PublishAsync(new RegisterUserNotification(user));

        var storedUser = await userRepository.TryGetAsync(_userRef);

        // Assert
        Assert.That(storedUser, Is.Not.Null);
        Assert.That(storedUser!.Name, Is.EqualTo("Alice"));
    }

    [Test]
    public async Task RegisterUser_WhenUpdate_UpdatesUserInMemory()
    {
        // Arrange
        var user = new User(_userRef, "Alice", "alice@example.com", DateTimeOffset.UtcNow, UserStatus.Offline);

        IStorage<UserRef, User> userRepository = _serviceProvider.GetRequiredService<IStorage<UserRef, User>>();
        await userRepository.SetAsync(user);

        RegisterUserNotificationHandler notificationHandler = new(
            LoggerFactory
                .Create(builder => builder.SetMinimumLevel(LogLevel.None))
                .CreateLogger<RegisterUserNotificationHandler>(),
            userRepository
        );

        var mediator = new ChatMediator([], [notificationHandler]);

        // Act
        user = user with { Name = "Alice Updated" };
        await mediator.PublishAsync(new RegisterUserNotification(user));

        var storedUser = await userRepository.TryGetAsync(_userRef);

        // Assert
        Assert.That(storedUser, Is.Not.Null);
        Assert.That(storedUser!.Name, Is.EqualTo("Alice Updated"));
    }
}


