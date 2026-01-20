using Mediators.Models;
using Mediators.Repository;
using Mediators.Repository.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.RegisterUserRepositories(builder.Configuration);
builder.Services.AddOpenApi();

var app = builder.Build();

// Ensure database is created on startup
await app.Services.EnsureDatabaseCreatedAsync();

app.MapOpenApi();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/users", async (CreateUserRequest request, IStorage<UserRef, User> userStorage) =>
{
    var user = new User(new UserRef { Id = request.Id ?? Guid.NewGuid() }, request.Name, request.Email, DateTimeOffset.UtcNow, UserStatus.Offline);
    await userStorage.SetAsync(user);
    return Results.Created($"/users/{user.Id.Id}", user);
});

app.MapGet("/users", async (IStorage<UserRef, User> userStorage) => Results.Ok(await userStorage.GetAllAsync()));

app.MapGet("/users/{id:guid}", async (Guid id, IStorage<UserRef, User> userStorage) =>
{
    var user = await userStorage.TryGetAsync(new UserRef { Id = id });
    return user is null ? Results.NotFound() : Results.Ok(user);
});

app.MapPut("/users/{id:guid}/status", async (Guid id, UpdateStatusRequest request, IStorage<UserRef, User> userStorage) =>
{
    var userRef = new UserRef { Id = id };
    var user = await userStorage.TryGetAsync(userRef);
    if (user is null)
    {
        return Results.NotFound();
    }

    var updated = user with { Status = request.Status, LastActiveTime = DateTimeOffset.UtcNow };
    await userStorage.SetAsync(updated);
    return Results.Ok(updated);
});

app.MapGet("/users/{id:guid}/notifications", async (Guid id, IStorage<UserRef, UserNotification> notificationStorage) =>
{
    var notification = await notificationStorage.TryGetAsync(new UserRef { Id = id });
    return notification is null ? Results.NotFound() : Results.Ok(notification);
});

app.MapPost(
    "/users/{id:guid}/notify",
    async (
        Guid id,
        NotifyUserRequest request,
        IStorage<UserRef, User> userStorage,
        IStorage<UserRef, UserNotification> notificationStorage,
        ILoggerFactory loggerFactory) =>
    {
        var logger = loggerFactory.CreateLogger("UserNotifications");
        var userRef = new UserRef { Id = id };
        var user = await userStorage.TryGetAsync(userRef);
        if (user is null)
        {
            return Results.NotFound();
        }

        var current = await notificationStorage.TryGetAsync(userRef) ?? new UserNotification(userRef, 0);
        var updated = current with { MessageCount = current.MessageCount + 1 };
        await notificationStorage.SetAsync(updated);

        if (user.Status == UserStatus.Offline)
        {
            logger.LogInformation(
                "[EMAIL] To: {Email}, Subject: New Message, Body: {Body}",
                user.Email,
                request.Message);
            logger.LogInformation(
                "[SMS] To user {UserId}: New message from {Sender}",
                user.Id.Id,
                request.SenderId);
        }
        else
        {
            logger.LogInformation(
                "[PUSH] To user {UserId}: {Body}",
                user.Id.Id,
                request.Message);
        }

        return Results.Ok(updated);
    });

app.MapPost("/reset", async (
    IStorage<UserRef, User> userStorage,
    IStorage<UserRef, UserNotification> notificationStorage) =>
{
    await userStorage.ClearAsync();
    await notificationStorage.ClearAsync();
    return Results.Ok(new { status = "cleared" });
});

app.Run();

internal sealed record CreateUserRequest(string Name, string Email, Guid? Id);
internal sealed record UpdateStatusRequest(UserStatus Status);
internal sealed record NotifyUserRequest(Guid SenderId, string Message);
