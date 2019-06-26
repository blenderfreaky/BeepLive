using System.Collections.Generic;
using System.Xml.Serialization;
using SFML.System;

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