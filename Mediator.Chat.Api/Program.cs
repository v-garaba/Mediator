using Mediators.Models;
using Mediators.Repository;
using Mediators.Repository.EntityFramework;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.RegisterChatRepositories(builder.Configuration);
builder.Services.AddOpenApi();

var app = builder.Build();

// Ensure database is created on startup
await app.Services.EnsureDatabaseCreatedAsync();

app.MapOpenApi();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/messages", async (CreateMessageRequest request, IStorage<MessageRef, ChatMessage> messageStorage) =>
{
    var message = new ChatMessage(
        new UserRef { Id = request.SenderId },
        request.Content,
        request.Type,
        request.TargetUserId is null ? null : new UserRef { Id = request.TargetUserId.Value }
    );

    await messageStorage.SetAsync(message);
    return Results.Created($"/messages/{message.Id.Id}", message);
});

app.MapGet("/messages", async (IStorage<MessageRef, ChatMessage> messageStorage) => Results.Ok(await messageStorage.GetAllAsync()));

app.MapGet("/messages/by-user/{userId:guid}", async (Guid userId, IStorage<MessageRef, ChatMessage> messageStorage) =>
{
    var messages = await messageStorage.GetAllAsync();
    var filtered = messages.Where(m => m.SenderId.Id == userId).ToList();
    return Results.Ok(filtered);
});

app.MapGet("/messages/count", async (IStorage<MessageRef, ChatMessage> messageStorage) =>
{
    var count = await messageStorage.CountAsync();
    return Results.Ok(new { count });
});

app.MapPost("/reset", async (IStorage<MessageRef, ChatMessage> messageStorage) =>
{
    await messageStorage.ClearAsync();
    return Results.Ok(new { status = "cleared" });
});

app.Run();

internal sealed record CreateMessageRequest(Guid SenderId, string Content, MessageType Type, Guid? TargetUserId);
