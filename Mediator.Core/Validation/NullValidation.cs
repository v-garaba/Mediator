namespace Mediators;

public static class ArgumentExtensions
{
    public static T AssertNotNull<T>(this T? argument)
    {
        ArgumentNullException.ThrowIfNull(argument);
        return argument;
    }
}


