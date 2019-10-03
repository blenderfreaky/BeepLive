namespace BeepLive
{
    using SFML.System;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal static class Linq
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T element in enumerable) action(element);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f Normalized(this Vector2f vector) => vector / vector.Magnitude();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Magnitude(this Vector2f vector) => MathF.Sqrt(vector.MagnitudeSquared());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float MagnitudeSquared(this Vector2f vector) => (vector.X * vector.X) + (vector.Y * vector.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f MultiplyByScalar(this Vector2f vector, float scalar) => new Vector2f(vector.X * scalar, vector.Y * scalar);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2f DivideByScalar(this Vector2f vector, float scalar) => new Vector2f(vector.X / scalar, vector.Y / scalar);
    }
}