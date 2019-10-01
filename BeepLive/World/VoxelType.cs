namespace BeepLive.World
{
    using BeepLive.Game;
    using SFML.Graphics;
    using System.Xml.Serialization;

    public class VoxelType
    {
        public Color Color;

        [XmlIgnore] public Team OwnerTeam;

        public float Resistance;
    }
}