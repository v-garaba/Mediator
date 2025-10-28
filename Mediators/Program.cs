using Mediators.Clients;
using Mediators.Messaging;
using Mediators.Messaging.Requests;
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
            .AddSingleton<ChatMediator>()
            .AddSingleton<ChatRoomService>()
            .AddSingleton<ChatRoom>()
            .BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        // Run services
        serviceProvider.GetRequiredService<EmailService>();
        serviceProvider.GetRequiredService<SmsService>();
        serviceProvider.GetRequiredService<PushNotificationService>();
        var analytics = serviceProvider.GetRequiredService<AnalyticsService>();
        serviceProvider.GetRequiredService<MessageStorageService>();
        serviceProvider.GetRequiredService<UserManagementService>();
        serviceProvider.GetRequiredService<NotificationService>();
        serviceProvider.GetRequiredService<ChatRoomService>();

        var chatRoom = serviceProvider.GetRequiredService<ChatRoom>();

        logger.LogInformation("=== Chat Room Application Started ===\n");

        // Create users
        var alice = new User(
            new UserRef(),
            "Alice",
            "alice@example.com",
            DateTime.MinValue,
            UserStatus.Offline
        );
        
        var bob = new User(
            new UserRef(),
            "Bob",
            "bob@example.com",
            DateTime.MinValue,
            UserStatus.Offline);

        var charlie = new User(
            new UserRef(),
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
        await chatRoom.ChangeUserStatusAsync(alice.Id, UserStatus.Online).ConfigureAwait(false);
        await chatRoom.ChangeUserStatusAsync(bob.Id, UserStatus.Online).ConfigureAwait(false);
        await chatRoom.ChangeUserStatusAsync(charlie.Id, UserStatus.Away).ConfigureAwait(false);

        Console.WriteLine("\n--- User Statuses Changed ---\n");

        // Send public messages
        await chatRoom
            .SendMessageAsync(alice.Id, "Hello everyone!", MessageType.Public)
            .ConfigureAwait(false);
        await chatRoom.SendMessageAsync(bob.Id, "Hi Alice!", MessageType.Public).ConfigureAwait(false);

        Console.WriteLine("\n--- Public Messages Sent ---\n");

        // Send private message
        await chatRoom
            .SendMessageAsync(alice.Id, "Hey Bob, how are you?", MessageType.Private, bob.Id)
            .ConfigureAwait(false);

        Console.WriteLine("\n--- Private Message Sent ---\n");

        // Change status to offline
        await chatRoom.ChangeUserStatusAsync(charlie.Id, UserStatus.Offline).ConfigureAwait(false);

        Console.WriteLine("\n--- Charlie went offline ---\n");

        // Send message to offline user
        await chatRoom
            .SendMessageAsync(alice.Id, "Charlie, see you later!", MessageType.Private, charlie.Id)
            .ConfigureAwait(false);

        Console.WriteLine("\n--- Message sent to offline user ---\n");

        var mediator = serviceProvider.GetRequiredService<ChatMediator>();
        var getMessageResponse1 = await mediator
            .Send(new GetMessageCountRequest(alice.Id))
            .ConfigureAwait(false);
        var getMessageResponse2 = await mediator
            .Send(new GetMessageCountRequest(bob.Id))
            .ConfigureAwait(false);
        var getMessageResponse3 = await mediator
            .Send(new GetMessageCountRequest(charlie.Id))
            .ConfigureAwait(false);
        logger.LogInformation($"\nAlice received {getMessageResponse1.Count} notifications");
        logger.LogInformation($"Bob received {getMessageResponse2.Count} notifications");
        logger.LogInformation($"Charlie received {getMessageResponse3.Count} notifications");

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
