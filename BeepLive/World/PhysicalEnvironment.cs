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
            VoxelTypes = new List<VoxelType>
            {
                new VoxelType{Name = "Ground", Color = new Color(127, 127, 127), Resistance = 0.1f}
            };
        }

        public VoxelType GetVoxelTypeByName(string name)
        {
            return VoxelTypes.Find(t => t.Name == name);
        }
    }
}