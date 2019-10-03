namespace BeepLive.World
{
    using SFML.System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class PhysicalEnvironment
    {
        public float AirResistance;
        public CollisionResponseMode CollisionResponseMode;
        public Vector2f Gravity;
        public float MovementThreshold;
        public int MovementThresholdMinDuration;

        [XmlIgnore] public List<VoxelType> VoxelTypes;

        public PhysicalEnvironment()
        {
            VoxelTypes = new List<VoxelType>();
        }
    }
}