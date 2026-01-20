namespace Mediators.Repository.EntityFramework.Entities;

/// <summary>
/// Entity Framework entity for UserNotification
/// </summary>
public class UserNotificationEntity
{
    public Guid UserId { get; set; }
    public int MessageCount { get; set; }
}
