using Mediators.Models;
using NUnit.Framework;

namespace Mediators.Tests.Models;

public class UserTests
{
    [Test]
    public void User_Creation_ShouldInitializeWithCorrectValues()
    {
        // Arrange & Act
        var user = new User("1", "Alice", "alice@example.com", DateTime.MinValue, UserStatus.Offline);

        // Assert
        Assert.That(user.Id, Is.EqualTo("1"));
        Assert.That(user.Name, Is.EqualTo("Alice"));
        Assert.That(user.Email, Is.EqualTo("alice@example.com"));
        Assert.That(user.Status, Is.EqualTo(UserStatus.Offline));
    }

    [Test]
    public void User_StatusChange_ShouldUpdateCorrectly()
    {
        // Arrange
        var user = new User("1", "Alice", "alice@example.com", DateTime.MinValue, UserStatus.Offline);

        // Act
        user = user with { Status = UserStatus.Online };

        // Assert
        Assert.That(user.Status, Is.EqualTo(UserStatus.Online));
    }

    [Test]
    public void User_LastActiveTime_CanBeUpdated()
    {
        // Arrange
        var user = new User("1", "Alice", "alice@example.com", DateTime.MinValue, UserStatus.Offline);
        var newTime = DateTime.UtcNow.AddHours(-1);

        // Act
        user = user with { LastActiveTime = newTime };

        // Assert
        Assert.That(user.LastActiveTime, Is.EqualTo(newTime));
    }

    [Test]
    public void UserStatus_Enum_HasAllExpectedValues()
    {
        // Assert
        Assert.That(Enum.IsDefined(typeof(UserStatus), UserStatus.Online), Is.True);
        Assert.That(Enum.IsDefined(typeof(UserStatus), UserStatus.Offline), Is.True);
        Assert.That(Enum.IsDefined(typeof(UserStatus), UserStatus.Away), Is.True);
        Assert.That(Enum.IsDefined(typeof(UserStatus), UserStatus.Busy), Is.True);
    }
}
