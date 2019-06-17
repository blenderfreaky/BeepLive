﻿using System.Xml.Serialization;
using SFML.Graphics;

namespace BeepLive.World
{
    public class VoxelType
    {
        public Color Color;
        [XmlIgnore]
        public Team OwnerTeam;
        public float Resistance;
    }
}