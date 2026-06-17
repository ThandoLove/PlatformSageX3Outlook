namespace OperationalWorkspaceShared.Helpers;

public static class FileSizeHelper
{
    public static string Format(long bytes)
    {
        string[] suffixes =
        {
            "B",
            "KB",
            "MB",
            "GB"
        };

        double size = bytes;

        int index = 0;

        while (size >= 1024 && index < suffixes.Length - 1)
        {
            size /= 1024;

            index++;
        }

        return $"{size:0.##} {suffixes[index]}";
    }
}