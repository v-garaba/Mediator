namespace Mediators.Models;

/// <summary>
/// Represents a user in the chat system
/// </summary>
public class User
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
    public UserStatus Status { get; set; }
    public DateTime LastActiveTime { get; set; }

    public User(string id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
        Status = UserStatus.Offline;
        LastActiveTime = DateTime.UtcNow;
    }
}

public enum UserStatus
{
    Online,
    Away,
    Busy,
    Offline
}
