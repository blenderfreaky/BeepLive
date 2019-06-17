using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using SFML.Graphics;
using SFML.System;

namespace BeepLive.World
{
    public class PhysicalEnvironment
    {
        public float AirResistance;
        public CollisionResponseMode CollisionResponseMode;
        public Vector2f Gravity;

        public List<VoxelType> VoxelTypes;

        public PhysicalEnvironment()
        {
            VoxelTypes = new List<VoxelType>();
        }
    }
}