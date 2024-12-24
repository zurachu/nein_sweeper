using System;
using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtension
{
    public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> self)
    {
        return self.Select((x, i) => (x, i));
    }

    public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
    {
        foreach (T item in self)
        {
            action?.Invoke(item);
        }
    }

    public static IOrderedEnumerable<T> Shuffle<T>(this IEnumerable<T> self)
    {
        return self.OrderBy(x => Guid.NewGuid());
    }
}
