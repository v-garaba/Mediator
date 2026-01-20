namespace Mediators.Repository.EntityFramework.Entities;

/// <summary>
/// Entity Framework entity for UserStatusChange
/// </summary>
public class UserStatusChangeEntity
{
    public Guid UserId { get; set; }
    public int NewStatus { get; set; }
}
