
namespace OperationalWorkspaceShared.Utilities;

public static class DateTimeUtility
{
    public static DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }

    public static string Format(DateTime date)
    {
        return date.ToString("yyyy-MM-dd HH:mm:ss");
    }
}