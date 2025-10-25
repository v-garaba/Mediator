using Mediators.Messaging;
using Mediators.Models;
using Mediators.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mediators;

class Program
{
    static async Task Main()
    {
        // Setup dependency injection
        var serviceProvider = new ServiceCollection()
            .AddLogging(configure => configure.AddConsole().SetMinimumLevel(LogLevel.Information))
            .AddSingleton<EmailService>()
            .AddSingleton<SmsService>()
            .AddSingleton<PushNotificationService>()
            .AddSingleton<AnalyticsService>()
            .AddSingleton<MessageStorageService>()
            .AddSingleton<UserManagementService>()
            .AddSingleton<NotificationService>()
            .AddSingleton<MessageBus>()
            .AddSingleton<ChatRoomService>()
            .BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        // Run services
        serviceProvider.GetRequiredService<EmailService>();
        serviceProvider.GetRequiredService<SmsService>();
        serviceProvider.GetRequiredService<PushNotificationService>();
        serviceProvider.GetRequiredService<AnalyticsService>();
        serviceProvider.GetRequiredService<MessageStorageService>();
        serviceProvider.GetRequiredService<UserManagementService>();
        serviceProvider.GetRequiredService<NotificationService>();

        var chatRoom = serviceProvider.GetRequiredService<ChatRoomService>();

        logger.LogInformation("=== Chat Room Application Started ===\n");

        // Create users
        var alice = new User(
            "1",
            "Alice",
            "alice@example.com",
            DateTime.MinValue,
            UserStatus.Offline
        );
        var bob = new User("2", "Bob", "bob@example.com", DateTime.MinValue, UserStatus.Offline);
        var charlie = new User(
            "3",
            "Charlie",
            "charlie@example.com",
            DateTime.MinValue,
            UserStatus.Offline
        );

        // Add users to chat room
        await chatRoom.AddUserAsync(alice).ConfigureAwait(false);
        await chatRoom.AddUserAsync(bob).ConfigureAwait(false);
        await chatRoom.AddUserAsync(charlie).ConfigureAwait(false);

        Console.WriteLine("\n--- Users Joined ---\n");

        // Change user statuses
        await chatRoom.ChangeUserStatusAsync("1", UserStatus.Online).ConfigureAwait(false);
        await chatRoom.ChangeUserStatusAsync("2", UserStatus.Online).ConfigureAwait(false);
        await chatRoom.ChangeUserStatusAsync("3", UserStatus.Away).ConfigureAwait(false);

        Console.WriteLine("\n--- User Statuses Changed ---\n");

        // Send public messages
        await chatRoom
            .SendMessageAsync("1", "Hello everyone!", MessageType.Public)
            .ConfigureAwait(false);
        await chatRoom.SendMessageAsync("2", "Hi Alice!", MessageType.Public).ConfigureAwait(false);

        Console.WriteLine("\n--- Public Messages Sent ---\n");

        // Send private message
        await chatRoom
            .SendMessageAsync("1", "Hey Bob, how are you?", MessageType.Private, "2")
            .ConfigureAwait(false);

        Console.WriteLine("\n--- Private Message Sent ---\n");

        // Change status to offline
        await chatRoom.ChangeUserStatusAsync("3", UserStatus.Offline).ConfigureAwait(false);

        Console.WriteLine("\n--- Charlie went offline ---\n");

        // Send message to offline user
        await chatRoom
            .SendMessageAsync("1", "Charlie, see you later!", MessageType.Private, "3")
            .ConfigureAwait(false);

        Console.WriteLine("\n--- Message sent to offline user ---\n");

        var analytics = serviceProvider.GetRequiredService<AnalyticsService>();
        logger.LogInformation($"\nAlice received {analytics.GetMessageCount("1")} notifications");
        logger.LogInformation($"Bob received {analytics.GetMessageCount("2")} notifications");
        logger.LogInformation($"Charlie received {analytics.GetMessageCount("3")} notifications");

        logger.LogInformation("\n=== Chat Room Application Finished ===");
        logger.LogInformation("\nPROBLEMS WITH CURRENT DESIGN:");
        logger.LogInformation("1. Tight coupling between services");
        logger.LogInformation("2. Too many dependencies in constructors");
        logger.LogInformation("3. Hard to test individual components");
        logger.LogInformation("4. Difficult to add new notification types");
        logger.LogInformation("5. Business logic scattered across multiple classes");
        logger.LogInformation("\nSOLUTION: Implement the Mediator Design Pattern!");
    }
}
