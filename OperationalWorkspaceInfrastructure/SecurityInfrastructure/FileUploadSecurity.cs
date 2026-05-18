using System.Text.RegularExpressions;

namespace OperationalWorkspaceInfrastructure.SecurityInfrastructure;

public static class FileUploadSecurity
{
    // ======================================================
    // ALLOWED EXTENSIONS
    // ======================================================

    private static readonly HashSet<string> AllowedExtensions =
    [
        ".pdf",
        ".doc",
        ".docx",
        ".xls",
        ".xlsx",
        ".png",
        ".jpg",
        ".jpeg",
        ".txt"
    ];

    // ======================================================
    // MAX FILE SIZE (10MB)
    // ======================================================

    private const long MaxFileSize = 10 * 1024 * 1024;

    // ======================================================
    // VALIDATE FILE
    // ======================================================

    public static void ValidateFile(
        string fileName,
        long fileSize)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new InvalidOperationException(
                "File name is required.");
        }

        if (fileSize <= 0)
        {
            throw new InvalidOperationException(
                "File is empty.");
        }

        if (fileSize > MaxFileSize)
        {
            throw new InvalidOperationException(
                "File exceeds maximum allowed size.");
        }

        var extension =
            Path.GetExtension(fileName)
                .ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException(
                $"File type '{extension}' is not allowed.");
        }
    }

    // ======================================================
    // SANITIZE FILE NAME
    // ======================================================

    public static string SanitizeFileName(
        string fileName)
    {
        // Remove path traversal
        fileName = Path.GetFileName(fileName);

        // Remove invalid chars
        fileName = Regex.Replace(
            fileName,
            @"[^a-zA-Z0-9_\.-]",
            "_");

        return fileName;
    }
}