using Mediators.Models;
using Mediators.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mediators;

class Program
{
    static void Main(string[] args)
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
            .AddSingleton<ChatRoomService>()
            .BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var chatRoom = serviceProvider.GetRequiredService<ChatRoomService>();

        logger.LogInformation("=== Chat Room Application Started ===\n");

        // Create users
        var alice = new User("1", "Alice", "alice@example.com");
        var bob = new User("2", "Bob", "bob@example.com");
        var charlie = new User("3", "Charlie", "charlie@example.com");

        // Add users to chat room
        chatRoom.AddUser(alice);
        chatRoom.AddUser(bob);
        chatRoom.AddUser(charlie);

        Console.WriteLine("\n--- Users Joined ---\n");

        // Change user statuses
        chatRoom.ChangeUserStatus("1", UserStatus.Online);
        chatRoom.ChangeUserStatus("2", UserStatus.Online);
        chatRoom.ChangeUserStatus("3", UserStatus.Away);

        Console.WriteLine("\n--- User Statuses Changed ---\n");

        // Send public messages
        chatRoom.SendMessage("1", "Hello everyone!", MessageType.Public);
        chatRoom.SendMessage("2", "Hi Alice!", MessageType.Public);

        Console.WriteLine("\n--- Public Messages Sent ---\n");

        // Send private message
        chatRoom.SendMessage("1", "Hey Bob, how are you?", MessageType.Private, "2");

        Console.WriteLine("\n--- Private Message Sent ---\n");

        // Change status to offline
        chatRoom.ChangeUserStatus("3", UserStatus.Offline);

        Console.WriteLine("\n--- Charlie went offline ---\n");

        // Send message to offline user
        chatRoom.SendMessage("1", "Charlie, see you later!", MessageType.Private, "3");

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
