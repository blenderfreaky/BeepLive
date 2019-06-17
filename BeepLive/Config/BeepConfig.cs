using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using BeepLive.World;

namespace BeepLive.Config
{
    public class BeepConfig
    {
        public float PlayerHealth;
        public PhysicalEnvironment PhysicalEnvironment;

        public int ChunkSize;
        public int MapWidth, MapHeight;

        public Boundary AroundChunkBoundary;

        public Color BackgroundColor;
    }
}
