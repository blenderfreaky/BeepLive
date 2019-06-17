using SFML.Graphics;

// ReSharper disable once CheckNamespace
namespace BeepLive.World
{
    public partial class Map
    {
        public Color BackgroundColor;
        public uint ChunkSize;
        public Boundary EntityBoundary;
        public int MapHeight;
        public int MapWidth;
        public PhysicalEnvironment PhysicalEnvironment;
    }
}