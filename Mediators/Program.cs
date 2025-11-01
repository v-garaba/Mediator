using Mediators.Clients;
using Mediators.Mediators;
using Mediators.Models;
using Mediators.NotificationHandlers;
using Mediators.Repository;
using Mediators.RequestHandlers;
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
            .RegisterRepositories()
            .RegisterRequestHandlers()
            .RegisterNotificationHandlers()
            .AddSingleton<IMediator, ChatMediator>()
            .AddSingleton<ChatRoom>()
            .BuildServiceProvider();

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var chatRoom = serviceProvider.GetRequiredService<ChatRoom>();

        logger.LogInformation("=== Chat Room Application Started ===\n");

        // Create users
        var alice = new User(
            new UserRef(),
            "Alice",
            "alice@example.com",
            DateTimeOffset.MinValue,
            UserStatus.Offline
        );

        var bob = new User(
            new UserRef(),
            "Bob",
            "bob@example.com",
            DateTimeOffset.MinValue,
            UserStatus.Offline);

        var charlie = new User(
            new UserRef(),
            "Charlie",
            "charlie@example.com",
            DateTimeOffset.MinValue,
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

        var mediator = serviceProvider.GetRequiredService<IMediator>();
        var getMessageResponse1 = await mediator
            .SendRequestAsync(new GetMessageCountRequest(alice.Id))
            .ConfigureAwait(false);
        var getMessageResponse2 = await mediator
            .SendRequestAsync(new GetMessageCountRequest(bob.Id))
            .ConfigureAwait(false);
        var getMessageResponse3 = await mediator
            .SendRequestAsync(new GetMessageCountRequest(charlie.Id))
            .ConfigureAwait(false);
        logger.LogInformation($"\nAlice received {getMessageResponse1.Count} notifications");
        logger.LogInformation($"Bob received {getMessageResponse2.Count} notifications");
        logger.LogInformation($"Charlie received {getMessageResponse3.Count} notifications");
    }
}
