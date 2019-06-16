using System;
using System.Collections.Generic;

namespace BeepLive
{
    internal static class Linq
    {
        public static TReturn Do<TInput, TReturn>(this TInput input, Func<TInput, TReturn> func) => func(input);
        public static void Do<TInput>(this TInput input, Action<TInput> func) => func(input);

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T element in enumerable)
            {
                action(element);
            }
        }

        public static T Evaluate<T>(this Func<T> func) => func();
        public static void Evaluate<T>(this Action action) => action();
    }

    internal static class Functional
    {
        public static Func<TResult> Lambda<TResult>(Func<TResult> func) => func;
        public static Func<T, TResult> Lambda<T, TResult>(Func<T, TResult> func) => func;
        public static Func<T1, T2, TResult> Lambda<T1, T2, TResult>(Func<T1, T2, TResult> func) => func;


        public static TResult Evaluate<TResult>(Func<TResult> func) => func();
        public static void Lambda(Action action) => action();
    }
}