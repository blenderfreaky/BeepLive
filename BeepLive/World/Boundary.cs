namespace BeepLive.World
{
    using SFML.System;
    using System;

    public struct Boundary
    {
        public Vector2f Min, Max;

        public bool Contains(Vector2f position)
        {
            return position.X >= Min.X && position.Y >= Min.Y && position.X <= Max.X && position.Y <= Max.Y;
        }

        public override bool Equals(object obj) => obj is Boundary boundary && Min.Equals(boundary.Min) && Max.Equals(boundary.Max);

        public override int GetHashCode() => HashCode.Combine(Min, Max);

        public static bool operator ==(Boundary left, Boundary right) => left.Equals(right);

        public static bool operator !=(Boundary left, Boundary right) => !(left == right);
    }
}