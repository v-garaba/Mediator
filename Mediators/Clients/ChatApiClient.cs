using System.Net.Http.Json;
using Mediators.Models;
using Microsoft.Extensions.Logging;

namespace Mediators.Clients;

public sealed class ChatApiClient(HttpClient httpClient, ILogger<ChatApiClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<ChatApiClient> _logger = logger;

    public async Task<ChatMessage> SendMessageAsync(
        UserRef senderId,
        string content,
        MessageType type,
        UserRef? targetUserId)
    {
        var request = new CreateMessageRequest(senderId.Id, content, type, targetUserId?.Id);
        var response = await _httpClient.PostAsJsonAsync("/messages", request).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var message = await response.Content.ReadFromJsonAsync<ChatMessage>().ConfigureAwait(false)
                      ?? throw new InvalidOperationException("Failed to deserialize message");
        _logger.LogInformation("[CHAT API] Sent message {MessageId}", message.Id.Id);
        return message;
    }

    public async Task<int> GetMessageCountAsync()
    {
        var response = await _httpClient.GetAsync("/messages/count").ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<MessageCountResponse>().ConfigureAwait(false)
                      ?? throw new InvalidOperationException("Failed to deserialize message count");
        return payload.count;
    }

    public async Task<IReadOnlyList<ChatMessage>> GetMessagesByUserAsync(UserRef userId)
    {
        var response = await _httpClient.GetAsync($"/messages/by-user/{userId.Id}").ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var messages = await response.Content.ReadFromJsonAsync<IReadOnlyList<ChatMessage>>().ConfigureAwait(false)
                       ?? Array.Empty<ChatMessage>();
        return messages;
    }

    public async Task ResetAsync()
    {
        var response = await _httpClient.PostAsync("/reset", null).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        _logger.LogInformation("[CHAT API] Reset complete");
    }

    private sealed record CreateMessageRequest(Guid SenderId, string Content, MessageType Type, Guid? TargetUserId);
    private sealed record MessageCountResponse(int count);
}
