using System.Xml.Serialization;
using BeepLive.Game;
using SFML.Graphics;

namespace BeepLive.World
{
    public class VoxelType
    {
        public Color Color;

        [XmlIgnore] public Team OwnerTeam;

        public float Resistance;
    }
}