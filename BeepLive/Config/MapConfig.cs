using System;
using System.Xml.Serialization;
using BeepLive.World;
using SFML.Graphics;
using SFML.System;

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
                    Max = value.Max - new Vector2f(MapWidth * ChunkSize, MapHeight * ChunkSize),
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
                    Max = value.Max + new Vector2f(MapWidth * ChunkSize, MapHeight * ChunkSize),
                };
            }
        }

        #region Fluent API

        public MapConfig SetSize(int mapWidth, int mapHeight)
        {
            if (mapWidth <= 0) throw new ArgumentOutOfRangeException(nameof(mapWidth));
            MapWidth = mapWidth;
            if (mapHeight <= 0) throw new ArgumentOutOfRangeException(nameof(mapHeight));
            MapHeight = mapHeight;

            return this;
        }

        public MapConfig SetChunkSize(uint chunkSize)
        {
            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
            ChunkSize = chunkSize;

            return this;
        }

        public MapConfig SetAirResistance(float airResistance)
        {
            PhysicalEnvironment.AirResistance = airResistance;

            return this;
        }

        public MapConfig SetGravity(float x, float y)
        {
            PhysicalEnvironment.Gravity = new Vector2f(x, y);

            return this;
        }

        public MapConfig SetCollisionResponseMode(CollisionResponseMode collisionResponseMode)
        {
            PhysicalEnvironment.CollisionResponseMode = collisionResponseMode;

            return this;
        }

        public MapConfig SetBackgroundColor(Color color)
        {
            BackgroundColor = color;

            return this;
        }

        public MapConfig SetEntityBoundary(Vector2f min, Vector2f max)
        {
            EntityBoundary = new Boundary {Min = min, Max = max};

            return this;
        }

        public MapConfig SetEntityBoundaryAroundChunks(Vector2f min, Vector2f max)
        {
            EntityBoundaryChunkOffset = new Boundary
            {
                Min = min,
                Max = max
            };

            return this;
        }

        #endregion
    }
}