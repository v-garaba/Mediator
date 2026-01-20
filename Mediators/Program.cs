using Mediators.Clients;
using Mediators.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mediators;

class Program
{
    static async Task Main()
    {
        await RunHttpModeAsync();
    }

    private static async Task RunHttpModeAsync()
    {
        var userApiBase = Environment.GetEnvironmentVariable("USER_API_BASE") ?? "http://localhost:61796";
        var chatApiBase = Environment.GetEnvironmentVariable("CHAT_API_BASE") ?? "http://localhost:61795";

        var services = new ServiceCollection()
            .AddLogging(configure => configure.AddConsole().SetMinimumLevel(LogLevel.Information));

        services.AddHttpClient<UserApiClient>(client => client.BaseAddress = new Uri(userApiBase));
        services.AddHttpClient<ChatApiClient>(client => client.BaseAddress = new Uri(chatApiBase));
        services.AddSingleton<IChatRoom, ChatRoom>();

        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var chatRoom = serviceProvider.GetRequiredService<IChatRoom>();
        var userClient = serviceProvider.GetRequiredService<UserApiClient>();
        var chatClient = serviceProvider.GetRequiredService<ChatApiClient>();

        logger.LogInformation("=== Chat Room Application Started (HTTP mode) ===\n");

        // Reset both services before running scenario
        logger.LogInformation("Resetting services...");
        await Task.WhenAll(userClient.ResetAsync(), chatClient.ResetAsync());
        logger.LogInformation("Services reset complete.\n");

        var (alice, bob, charlie) = await RunScenarioAsync(chatRoom);

        var totalCount = await chatClient.GetMessageCountAsync();
        logger.LogInformation("Total messages sent: {Count}", totalCount);

        var aliceNotif = await userClient.GetNotificationAsync(alice.Id);
        var bobNotif = await userClient.GetNotificationAsync(bob.Id);
        var charlieNotif = await userClient.GetNotificationAsync(charlie.Id);

        logger.LogInformation("Alice notification count: {Count}", aliceNotif?.MessageCount ?? 0);
        logger.LogInformation("Bob notification count: {Count}", bobNotif?.MessageCount ?? 0);
        logger.LogInformation("Charlie notification count: {Count}", charlieNotif?.MessageCount ?? 0);
    }

    private static async Task<(User alice, User bob, User charlie)> RunScenarioAsync(IChatRoom chatRoom)
    {
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

        await chatRoom.AddUserAsync(alice).ConfigureAwait(false);
        await chatRoom.AddUserAsync(bob).ConfigureAwait(false);
        await chatRoom.AddUserAsync(charlie).ConfigureAwait(false);

        Console.WriteLine("\n--- Users Joined ---\n");

        await chatRoom.ChangeUserStatusAsync(alice.Id, UserStatus.Online).ConfigureAwait(false);
        await chatRoom.ChangeUserStatusAsync(bob.Id, UserStatus.Online).ConfigureAwait(false);
        await chatRoom.ChangeUserStatusAsync(charlie.Id, UserStatus.Away).ConfigureAwait(false);

        Console.WriteLine("\n--- User Statuses Changed ---\n");

        await chatRoom
            .SendMessageAsync(alice.Id, "Hello everyone!", MessageType.Public)
            .ConfigureAwait(false);
        await chatRoom.SendMessageAsync(bob.Id, "Hi Alice!", MessageType.Public).ConfigureAwait(false);

        Console.WriteLine("\n--- Public Messages Sent ---\n");

        await chatRoom
            .SendMessageAsync(alice.Id, "Hey Bob, how are you?", MessageType.Private, bob.Id)
            .ConfigureAwait(false);

        Console.WriteLine("\n--- Private Message Sent ---\n");

        await chatRoom.ChangeUserStatusAsync(charlie.Id, UserStatus.Offline).ConfigureAwait(false);

        Console.WriteLine("\n--- Charlie went offline ---\n");

        await chatRoom
            .SendMessageAsync(alice.Id, "Charlie, see you later!", MessageType.Private, charlie.Id)
            .ConfigureAwait(false);

        Console.WriteLine("\n--- Message sent to offline user ---\n");

        return (alice, bob, charlie);
    }
}


