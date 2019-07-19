using SFML.System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BeepLive.World
{
    public class PhysicalEnvironment
    {
        public float AirResistance;
        public CollisionResponseMode CollisionResponseMode;
        public Vector2f Gravity;
        public float MovementThreshold;

        [XmlIgnore] public List<VoxelType> VoxelTypes;

        public PhysicalEnvironment()
        {
            VoxelTypes = new List<VoxelType>();
        }
    }
}