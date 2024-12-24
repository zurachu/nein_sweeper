using System.Collections.Generic;

public static class ListExtension
{
    public static T Pop<T>(this IList<T> self)
    {
        var result = self[0];
        self.RemoveAt(0);
        return result;
    }
}
