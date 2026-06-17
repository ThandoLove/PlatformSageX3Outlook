using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalWorkspaceShared.SharedExtensions;


public static class EnumerableExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T>? items)
    {
        return items == null || !items.Any();
    }
}