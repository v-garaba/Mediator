namespace Mediators.Repository.EntityFramework.Entities;

/// <summary>
/// Entity Framework entity for User
/// </summary>
public class UserEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTimeOffset LastActiveTime { get; set; }
    public int Status { get; set; }
}
