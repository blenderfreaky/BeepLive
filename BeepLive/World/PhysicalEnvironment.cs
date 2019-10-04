namespace BeepLive.World
{
    using SFML.Graphics;
    using SFML.System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Serialization;

    public class PhysicalEnvironment
    {
        public float AirResistance;
        public CollisionResponseMode CollisionResponseMode;
        public Vector2f Gravity;
        public float MovementThreshold;
        public int MovementThresholdMinDuration;

        [XmlIgnore] public List<VoxelType> VoxelTypes;
        [XmlIgnore] public Dictionary<Color, VoxelType> VoxelTypesByColor;

        public PhysicalEnvironment()
        {
            VoxelTypes = new List<VoxelType>();
        }

        public void CalculateVoxelTypesByColor() => VoxelTypesByColor = VoxelTypes.ToDictionary(x => x.Color, x => x);
    }
}