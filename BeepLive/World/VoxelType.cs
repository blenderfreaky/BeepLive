using BeepLive.Game;
using SFML.Graphics;
using System.Xml.Serialization;

namespace BeepLive.World
{
    public class VoxelType
    {
        public Color Color;

        [XmlIgnore] public Team OwnerTeam;

        public float Resistance;
    }
}