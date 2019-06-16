using System.ComponentModel;
using SFML.System;

namespace BeepLive.World
{
    public class PhysicalEnvironment
    {
        public float AirResistance;
        public Vector2f Gravity;
        public CollisionResponseMode CollisionResponseMode;
    }

    public enum CollisionResponseMode
    {
        [Description("")]
        Panic,
        [Description("Move object up vertically")]
        Raise,
        [Description("Move object to direction with least collisions")]
        LeastResistance,
    }
}