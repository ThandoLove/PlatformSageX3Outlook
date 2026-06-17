using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceShared.SharedExtensions;


public static class StringExtensions
{
    public static bool HasValue(this string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }

    public static string Safe(this string? value)
    {
        return value ?? string.Empty;
    }
}