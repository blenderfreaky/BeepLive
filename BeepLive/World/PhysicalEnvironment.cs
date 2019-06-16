using System.Collections.Generic;
using SFML.System;

namespace BeepLive.World
{
    public class PhysicalEnvironment
    {
        public float AirResistance;
        public Vector2f Gravity;
        public CollisionResponseMode CollisionResponseMode;

        public List<VoxelType> VoxelTypes;

        public PhysicalEnvironment()
        {
            VoxelTypes = new List<VoxelType>();
        }
    }
}