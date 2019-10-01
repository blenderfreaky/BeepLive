namespace BeepLive
{
    using System;
    using System.Collections.Generic;

    internal static class Linq
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T element in enumerable) action(element);
        }
    }
}