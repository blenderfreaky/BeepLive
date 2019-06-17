using System;
using System.Collections.Generic;
using BeepLive.Entities;
using SFML.Graphics;
using SFML.System;
using SimplexNoise;

namespace BeepLive.World
{
    public class Map
    {
        public Color BackgroundColor;
        public Chunk[,] Chunks;
        public uint ChunkSize;
        public List<Entity> Entities;
        public List<Player> Players;
        public Boundary EntityBoundary;
        public int MapWidth, MapHeight;
        public PhysicalEnvironment PhysicalEnvironment;
        public Random Random;

        public Map()
        {
            PhysicalEnvironment = new PhysicalEnvironment();

            Entities = new List<Entity>();
            Players =new List<Player>();

            Random = new Random();
        }

        public void Step()
        {
            lock (Entities)
            {
                // Make array to avoid concurrent modification exception; Make temporary clone to be able to modify the original
                Entities.ToArray().ForEach(e => e.Step());
            }
        }

        public Vector2i GetChunkIndex(Vector2f position)
        {
            return new Vector2i((int) MathF.Floor(position.X), (int) MathF.Floor(position.Y));
        }

        public Chunk GetChunk(Vector2f position, out Vector2f chunkPosition)
        {
            int i = (int) MathF.Floor(position.X / ChunkSize);
            int j = (int) MathF.Floor(position.Y / ChunkSize);
            chunkPosition = new Vector2f(i * ChunkSize, j * ChunkSize);
            return i < 0 || j < 0 || i >= MapWidth || j >= MapHeight ? null : Chunks[i, j];
        }

        public Voxel GetVoxel(Vector2f position)
        {
            return GetChunk(position, out Vector2f chunkPosition)?.GetVoxel(position - chunkPosition) ?? new Voxel(this);
        }

        #region Fluent API

        public Map SetSize(int mapWidth, int mapHeight)
        {
            if (mapWidth <= 0) throw new ArgumentOutOfRangeException(nameof(mapWidth));
            MapWidth = mapWidth;
            if (mapHeight <= 0) throw new ArgumentOutOfRangeException(nameof(mapHeight));
            MapHeight = mapHeight;
            Chunks = new Chunk[mapWidth, mapHeight];

            return this;
        }

        public Map SetChunkSize(uint chunkSize)
        {
            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));
            ChunkSize = chunkSize;

            return this;
        }

        public Map SetAirResistance(float airResistance)
        {
            PhysicalEnvironment.AirResistance = airResistance;

            return this;
        }

        public Map SetGravity(float x, float y)
        {
            PhysicalEnvironment.Gravity = new Vector2f(x, y);

            return this;
        }

        public Map SetCollisionResponseMode(CollisionResponseMode collisionResponseMode)
        {
            PhysicalEnvironment.CollisionResponseMode = collisionResponseMode;

            return this;
        }

        public Map SetBackgroundColor(Color color)
        {
            BackgroundColor = color;

            return this;
        }

        public Map SetEntityBoundary(Vector2f min, Vector2f max)
        {
            EntityBoundary = new Boundary {Min = min, Max = max};

            return this;
        }

        public Map SetEntityBoundaryAroundChunks(Vector2f min, Vector2f max)
        {
            EntityBoundary = new Boundary
            {
                Min = min,
                Max = new Vector2f(MapWidth * ChunkSize, MapHeight * ChunkSize) + max
            };

            return this;
        }

        public Map GenerateMap(VoxelType ground, int groundLevel, float scale, float heightScale)
        {
            PhysicalEnvironment.VoxelTypes.Add(ground);

            for (uint chunkI = 0; chunkI < MapWidth; chunkI++)
            for (uint chunkJ = 0; chunkJ < MapHeight; chunkJ++)
            {
                Chunk chunk = Chunks[chunkI, chunkJ] =
                    new Chunk(this, new Vector2f(chunkI * ChunkSize, chunkJ * ChunkSize));

                for (uint voxelI = 0; voxelI < ChunkSize; voxelI++)
                {
                    float height = Noise.CalcPixel1D(
                                     (int) (chunkI * ChunkSize + voxelI),
                                     scale) * heightScale / 128f;

                    for (uint voxelJ = 0; voxelJ < ChunkSize; voxelJ++)
                    {
                        bool isAir = chunkJ * ChunkSize + voxelJ - groundLevel < height;

                        chunk[voxelI, voxelJ] =
                            isAir
                                ? new Voxel(this)
                                : new Voxel(this, ground);
                    }
                }
            }

            return this;
        }

        public Map AddPlayer(Vector2f position, int size)
        {
            Entities.Add(new Player(this, position, size));

            return this;
        }

        public Map AddProjectile(Vector2f position, Vector2f velocity, float radius, float lowestSpeed, int maxLifeTime)
        {
            Entities.Add(new Projectile(this, position, velocity, radius, lowestSpeed, maxLifeTime));

            return this;
        }

        public Map AddClusterProjectile(Vector2f position, Vector2f velocity, float radius, float lowestSpeed,
            int maxLifeTime,
            int childCount,
            int childRadius, float explosionPower, float childLowestSpeed, int childMaxLifeTime)
        {
            Entities.Add(new ClusterProjectile<Projectile>(this, position, velocity, radius, lowestSpeed, maxLifeTime,
                childCount, childRadius, explosionPower, childLowestSpeed, childMaxLifeTime));

            return this;
        }

        #endregion
    }
}