using System;
using System.Collections.Generic;

namespace BeepLive
{
    internal static class Linq
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var element in enumerable) action(element);
        }
    }
}