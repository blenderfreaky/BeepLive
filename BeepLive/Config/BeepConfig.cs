using BeepLive.World;
using SFML.Graphics;

namespace BeepLive.Config
{
    public class BeepConfig
    {
        public Boundary AroundChunkBoundary;

        public Color BackgroundColor;

        public int ChunkSize;
        public int MapWidth, MapHeight;
        public PhysicalEnvironment PhysicalEnvironment;
        public float PlayerHealth;
    }
}