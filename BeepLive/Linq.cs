using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace BeepLive
{
    internal static class Linq
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T element in enumerable)
            {
                action(element);
            }
        }
    }

    internal static class Functional
    {
        public static Func<TResult> Lambda<TResult>(Func<TResult> func) => func;
        public static Func<T, TResult> Lambda<T, TResult>(Func<T, TResult> func) => func;
        public static Func<T1, T2, TResult> Lambda<T1, T2, TResult>(Func<T1, T2, TResult> func) => func;
        public static Action Lambda(Action action) => action;


        public static TResult Evaluate<TResult>(Func<TResult> func) => func();
        public static void Evaluate(Action action) => action();
    }
}