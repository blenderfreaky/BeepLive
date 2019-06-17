using SFML.System;

namespace BeepLive.World
{
    public struct Boundary
    {
        public Vector2f Min, Max;

        public bool Contains(Vector2f position)
        {
            return position.X >= Min.X && position.Y >= Min.Y && position.X <= Max.X && position.Y <= Max.Y;
        }
    }
}