using BeepLive.World;
using SFML.Graphics;
using SFML.System;
using System.Xml.Serialization;

namespace BeepLive.Config
{
    public class MapConfig
    {
        private Boundary _entityBoundary;

        private Boundary _entityBoundaryChunkOffset;
        public Color BackgroundColor;
        public uint ChunkSize;
        public float FloatingNoiseScale, FloatingNoiseThreshold, FloatingNoiseFalloff;
        public int GroundLevel;
        public VoxelType GroundVoxelType;
        public float HorizontalNoiseScale, VerticalNoiseScale;
        public int MapHeight;
        public int MapWidth;
        public PhysicalEnvironment PhysicalEnvironment;

        public MapConfig()
        {
            PhysicalEnvironment = new PhysicalEnvironment();
            GroundVoxelType = new VoxelType();
        }

        [XmlIgnore]
        public Boundary EntityBoundary
        {
            get => _entityBoundary;
            set
            {
                _entityBoundary = value;
                _entityBoundaryChunkOffset = new Boundary
                {
                    Min = value.Min,
                    Max = value.Max - new Vector2f(MapWidth * ChunkSize, MapHeight * ChunkSize)
                };
            }
        }

        public Boundary EntityBoundaryChunkOffset
        {
            get => _entityBoundaryChunkOffset;
            set
            {
                _entityBoundaryChunkOffset = value;
                _entityBoundary = new Boundary
                {
                    Min = value.Min,
                    Max = value.Max + new Vector2f(MapWidth * ChunkSize, MapHeight * ChunkSize)
                };
            }
        }
    }
}