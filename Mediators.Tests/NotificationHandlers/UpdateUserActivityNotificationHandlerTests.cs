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
internal sealed class UpdateUserActivityNotificationHandlerTests
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
    public async Task UpdateUserActivity_UpdatesUserLastActiveTime()
    {
        // Arrange
        var user = new User(_userRef, "Alice", "alice@example.com", DateTimeOffset.UtcNow.AddHours(-5), UserStatus.Offline);

        IStorage<UserRef, User> userRepository = _serviceProvider.GetRequiredService<IStorage<UserRef, User>>();
        await userRepository.SetAsync(user);

        UpdateUserActivityNotificationHandler notificationHandler = new(
            LoggerFactory
                .Create(builder => builder.SetMinimumLevel(LogLevel.None))
                .CreateLogger<UpdateUserActivityNotificationHandler>(),
            userRepository
        );

        var mediator = new ChatMediator([], [notificationHandler]);

        // Act
        var utcNowSnapshot = DateTimeOffset.UtcNow;
        await mediator.PublishAsync(new UpdateUserActivityNotification(_userRef));

        var storedUser = await userRepository.TryGetAsync(_userRef);

        // Assert
        Assert.That(storedUser, Is.Not.Null);
        Assert.That(storedUser!.LastActiveTime, Is.EqualTo(utcNowSnapshot).Within(TimeSpan.FromSeconds(1)));
    }
}


