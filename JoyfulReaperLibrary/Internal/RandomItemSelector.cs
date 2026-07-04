using System;
using System.Collections.Generic;

namespace JoyfulReaperLib.Internal;

internal static class RandomItemSelector
{
    public static T RandomItem<T>(IReadOnlyList<T> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (items.Count == 0)
        {
            throw new ArgumentException("Collection cannot be empty.", nameof(items));
        }

        return items[Random.Shared.Next(items.Count)];
    }
}
